using System;
using System.Collections.Generic;

namespace Application.DTOs.AI
{
    /// <summary>
    /// One consolidated, doctor-facing report combining every PatientResult's AI summary/report/
    /// suggestion for a patient, plus a single AI-synthesized overview across all of them.
    /// This is what the "gather all patient summaries and AI report into one report" endpoint returns.
    /// </summary>
    public class PatientFullAIReportDto
    {
        public int PatientId { get; set; }
        public string PatientFullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;

        public DateTime GeneratedAtUtc { get; set; }

        /// <summary>Per-lab-test AI analyses that make up this report, most recent first.</summary>
        public List<PatientResultAIAnalysisDto> Results { get; set; } = new();

        /// <summary>AI-synthesized overview considering trends/patterns across all of the patient's results together.</summary>
        public string OverallAISummary { get; set; } = string.Empty;

        /// <summary>AI-synthesized, cross-result clinical suggestions for the doctor to review.</summary>
        public string OverallAISuggestion { get; set; } = string.Empty;

        public string Disclaimer { get; set; } =
            "AI-generated decision support. Must be reviewed and confirmed by a licensed physician before any clinical action is taken.";
    }
}
