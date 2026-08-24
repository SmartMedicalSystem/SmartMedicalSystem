using Application.DTOs.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Application.Services.AI
{
    /// <summary>
    /// Pure, side-effect-free helpers for turning lab data into an AI prompt and parsing the
    /// model's JSON reply back into strongly-typed fields. Kept separate from the services so the
    /// prompt wording can be reviewed/tuned by a clinician without touching data-access code.
    /// </summary>
    public static class PatientResultPromptBuilder
    {
        public const string ResultAnalysisSystemPrompt =
            "You are a clinical decision-support assistant embedded in a hospital laboratory system. " +
            "You are given a single lab test's numeric results, already compared against their normal reference ranges. " +
            "Write for a licensed physician who will review and confirm everything you say - you are drafting a first pass, not a diagnosis. " +
            "Be factual, concise, and never invent values that were not provided. " +
            "Respond ONLY with a single minified JSON object, no markdown fences, no commentary, using exactly these keys: " +
            "\"summary\" (2-4 sentences in plain clinical language describing the result), " +
            "\"classifiedReport\" (a structured breakdown classifying each abnormal finding and its likely clinical significance), " +
            "\"suggestion\" (bullet-style next-step considerations for the physician, e.g. repeat testing, referral, correlate with symptoms). " +
            "Always end the \"suggestion\" field with a short reminder that this is AI-generated support requiring physician confirmation.";

        public const string FullReportSystemPrompt =
            "You are a clinical decision-support assistant embedded in a hospital laboratory system. " +
            "You are given a licensed physician's patient and a chronological list of that patient's already-analyzed lab results " +
            "(each with its own summary, classification and suggestion). " +
            "Synthesize them into a single cross-result overview: note trends over time, correlations between different tests, " +
            "and anything that stands out only when the results are viewed together. Do not repeat every individual result verbatim. " +
            "Respond ONLY with a single minified JSON object, no markdown fences, no commentary, using exactly these keys: " +
            "\"overallSummary\" and \"overallSuggestion\". " +
            "Always end \"overallSuggestion\" with a short reminder that this is AI-generated support requiring physician confirmation.";

        public const string ChatSystemPromptTemplate =
            "You are a clinical decision-support chatbot for physicians, answering questions about a specific patient's lab history. " +
            "Answer ONLY using the CONTEXT passages provided below - they are excerpts from that patient's previously generated AI lab summaries/reports. " +
            "If the context does not contain the answer, say so plainly instead of guessing. " +
            "Keep answers concise and clinically precise. Always remind the physician to verify against the primary chart.\n\n" +
            "CONTEXT:\n{0}";

        //public const string GroupChatSystemPromptTemplate =
        //   "You are a clinical decision-support chatbot for physicians, answering questions about a specific patient's lab history. " +
        //   "Answer ONLY using the CONTEXT passages provided below - they are excerpts from that patient's previously generated AI lab summaries/reports. " +
        //   "If the context does not contain the answer, say so plainly instead of guessing. " +
        //   "Keep answers concise and clinically precise. Always remind the physician to verify against the primary chart.\n\n" +
        //   "CONTEXT:\n{0} Dont't mention the pateint's id or personal data in the response ";

        public const string GroupChatSystemPromptTemplate = "You are a clinical decision-support chatbot for physicians, answering " +
            "questions about a specific patient's laboratory history.\r\n\r\nAnswer ONLY using the CONTEXT passages provided below. " +
            "The context contains excerpts from that patient's previously generated AI lab summaries/reports.\r\n\r\n### Privacy and Safety Rules\r\n\r\n* " +
            "NEVER mention, repeat, quote, or expose the patient's ID.\r\n* NEVER mention the patient's name or any other personally identifiable information (PII)." +
            "\r\n* Do NOT include identifiers even if they appear in the CONTEXT.\r\n* Do NOT infer or reconstruct personal information from the CONTEXT.\r\n* When answering, " +
            "refer to the person only as \"the patient\".\r\n* Focus only on the clinical/laboratory information necessary to answer the physician's question.\r\n\r\n### Answering " +
            "Rules\r\n\r\n* If the CONTEXT does not contain the answer, say so plainly instead of guessing.\r\n* Keep answers concise and clinically precise.\r\n* Do not introduce information " +
            "that is not supported by the CONTEXT.\r\n* Always remind the physician to verify the information against the primary chart.\r\n\r\nCONTEXT:\r\n{0}\r\n";
        public static string BuildResultAnalysisUserPrompt(string labTestName, IReadOnlyList<PatientResultElementSummaryDto> elements)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Lab test: {labTestName}");
            sb.AppendLine("Elements (name | value unit | normal range | flag):");

            foreach (var e in elements)
            {
                sb.AppendLine($"- {e.ElementName} | {e.Value} {e.Unit} | {e.NormalMin}-{e.NormalMax} {e.Unit} | {e.Flag}");
            }

            return sb.ToString();
        }

        public static string BuildFullReportUserPrompt(string patientFullName, int age, string gender,
            IReadOnlyList<PatientResultAIAnalysisDto> results)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Patient: {patientFullName}, age {age}, gender {gender}");
            sb.AppendLine($"Number of lab results on file: {results.Count}");
            sb.AppendLine();

            foreach (var r in results.OrderByDescending(r => r.GeneratedAtUtc))
            {
                sb.AppendLine($"[{r.GeneratedAtUtc:yyyy-MM-dd}] {r.LabTestName}");
                sb.AppendLine($"Summary: {r.Summary}");
                sb.AppendLine($"Classification: {r.AIClassifiedReport}");
                sb.AppendLine($"Suggestion: {r.AISuggestion}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string BuildChatSystemPrompt(IReadOnlyList<string> contextChunks)
        {
            var context = contextChunks.Count == 0
                ? "(no matching indexed documents were found for this query)"
                : string.Join("\n---\n", contextChunks.Select((c, i) => $"Passage {i + 1}:\n{c}"));

            return string.Format(ChatSystemPromptTemplate, context);
        }

        public static string BuildChatSystemPromptGroupedByPatient(IReadOnlyList<Application.DTOs.Rag.RagSourceDto> sources)
        {
            if (sources == null || sources.Count == 0)
                return string.Format(GroupChatSystemPromptTemplate, "(no matching indexed documents were found for this query)");

            var sb = new StringBuilder();

            var groups = sources.GroupBy(s => s.PatientId).OrderBy(g => g.Key).ToList();
            var patientIndex = 1;
            foreach (var g in groups)
            {
                // Use de-identified patient labels instead of exposing internal PatientId values
                sb.AppendLine($"Patient #{patientIndex} (de-identified):");
                var passages = g.Select((s, i) => $"- [{s.SourceType}] {s.Content.Replace('\n', ' ').Trim()}");
                foreach (var p in passages)
                {
                    sb.AppendLine(p);
                }
                sb.AppendLine();
                patientIndex++;
            }

            return string.Format(GroupChatSystemPromptTemplate, sb.ToString());
        }

        /// <summary>
        /// Parses a JSON object out of the model's raw text response, tolerating the common case of
        /// the model wrapping it in ```json ... ``` markdown fences despite being asked not to.
        /// </summary>
        public static Dictionary<string, string> ParseJsonObject(string rawModelOutput)
        {
            var text = rawModelOutput.Trim();

            if (text.StartsWith("```"))
            {
                var firstNewline = text.IndexOf('\n');
                if (firstNewline >= 0)
                    text = text[(firstNewline + 1)..];

                var fenceEnd = text.LastIndexOf("```", StringComparison.Ordinal);
                if (fenceEnd >= 0)
                    text = text[..fenceEnd];
            }

            text = text.Trim();

            var start = text.IndexOf('{');
            var end = text.LastIndexOf('}');
            if (start >= 0 && end > start)
                text = text[start..(end + 1)];

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using var doc = JsonDocument.Parse(text);
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    result[prop.Name] = prop.Value.ValueKind == JsonValueKind.String
                        ? prop.Value.GetString() ?? string.Empty
                        : prop.Value.GetRawText();
                }
            }
            catch (JsonException)
            {
                // The model didn't return valid JSON - fall back to surfacing the raw text as a
                // single field rather than throwing away everything it produced.
                result["raw"] = rawModelOutput;
            }

            return result;
        }
    }
}
