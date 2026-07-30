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
