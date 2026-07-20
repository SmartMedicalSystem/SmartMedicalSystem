namespace Application.DTOs.PatientResult
{

    using Application.DTOs.Patient;
    using Application.DTOs.Session;
    using Application.DTOs.LabTest;
    using Application.DTOs.PatientResultElement;

    public class PatientResultReadDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int SessionId { get; set; }
        public int LabTestId { get; set; }
        public string Summary { get; set; } = null!;
        public string AIClassifiedReport { get; set; } = null!;
        public string AISuggestion { get; set; } = null!;

        public string Notes { get; set; } = string.Empty;
        public bool IsDraft { get; set; } = false;

        // Nested read DTOs for UI display
        public PatientReadDto? Patient { get; set; }
        public SessionReadDto? Session { get; set; }
        public LabTestReadDto? LabTest { get; set; }

        public List<PatientResultElementReadDto> ResultElements { get; set; } = new();
    }
}
