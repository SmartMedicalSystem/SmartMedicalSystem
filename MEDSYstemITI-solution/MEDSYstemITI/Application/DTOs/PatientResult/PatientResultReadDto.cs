namespace Application.DTOs.PatientResult
{

    public class PatientResultReadDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int SessionId { get; set; }
        public int LabTestId { get; set; }
        public string Summary { get; set; } = null!;
        public string AIClassifiedReport { get; set; } = null!;
        public string AISuggestion { get; set; } = null!;
    }
}
