using System;
using System.Collections.Generic;

namespace Application.DTOs.AI
{
    /// <summary>Result of running AI summarization/classification/suggestion over one PatientResult.</summary>
    public class PatientResultAIAnalysisDto
    {
        public int PatientResultId { get; set; }
        public int PatientId { get; set; }
        public int SessionId { get; set; }
        public int LabTestId { get; set; }
        public string LabTestName { get; set; } = string.Empty;
        public DateTime GeneratedAtUtc { get; set; }

        /// <summary>Deterministic value-vs-range breakdown fed to the AI model as grounding context.</summary>
        public List<PatientResultElementSummaryDto> Elements { get; set; } = new();

        /// <summary>Short plain-language summary of the result (PatientResult.Summary).</summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>Structured clinical classification of the findings (PatientResult.AIClassifiedReport).</summary>
        public string AIClassifiedReport { get; set; } = string.Empty;

        /// <summary>AI-suggested next steps / clinical considerations (PatientResult.AISuggestion).</summary>
        public string AISuggestion { get; set; } = string.Empty;

        /// <summary>Always surfaced to the doctor alongside AI output.</summary>
        public string Disclaimer { get; set; } =
            "AI-generated decision support. Must be reviewed and confirmed by a licensed physician before any clinical action is taken.";
    }
}
