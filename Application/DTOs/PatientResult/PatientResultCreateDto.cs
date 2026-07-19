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
    }
}
