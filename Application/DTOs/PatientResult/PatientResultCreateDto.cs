namespace Application.DTOs.PatientResult
{
    public class PatientResultCreateDto
    {
        public int PatientId { get; set; }
        public int SessionId { get; set; }
        public int LabTestId { get; set; }
        public string Summary { get; set; } = null!;
        public string AIClassifiedReport { get; set; } = string.Empty;
        public string AISuggestion { get; set; } = string.Empty;

        // Optional laboratory notes
        public string Notes { get; set; } = string.Empty;

        // Save as draft without completing the request
        public bool IsDraft { get; set; } = false;

        // Collection of measurement elements to create with the result
        public List<ResultElementForCreate> ResultElements { get; set; } = new();

        public class ResultElementForCreate
        {
            public int TestElementId { get; set; }
            public double Value { get; set; }
            public int TechId { get; set; }
        }
    }
}
