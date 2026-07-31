namespace Application.DTOs.AI
{
    /// <summary>Deterministic (non-AI) comparison of one PatientResultElement against its reference range.</summary>
    public class PatientResultElementSummaryDto
    {
        public int TestElementId { get; set; }
        public string ElementName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public double Value { get; set; }
        public float NormalMin { get; set; }
        public float NormalMax { get; set; }

        /// <summary>"Low", "Normal", or "High" - computed in code, never guessed by the AI model.</summary>
        public string Flag { get; set; } = "Normal";
    }
}
