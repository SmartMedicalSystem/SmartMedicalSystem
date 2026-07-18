namespace Application.DTOs.PatientResult
{
    public class PatientResultUpdateDto
    {
        public string Summary { get; set; } = null!;
        public string AIClassifiedReport { get; set; } = string.Empty;
        public string AISuggestion { get; set; } = string.Empty;
    }
}
