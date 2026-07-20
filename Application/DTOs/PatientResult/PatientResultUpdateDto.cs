namespace Application.DTOs.PatientResult
{
    public class PatientResultUpdateDto
    {
        public string Summary { get; set; } = null!;
        public string AIClassifiedReport { get; set; } = string.Empty;
        public string AISuggestion { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsDraft { get; set; } = false;

        public List<ResultElementForUpdate> ResultElements { get; set; } = new();

        public class ResultElementForUpdate
        {
            public int Id { get; set; } // existing PatientResultElement Id; 0 for new items
            public int TestElementId { get; set; }
            public double Value { get; set; }
            public int TechId { get; set; }
        }
    }
}
