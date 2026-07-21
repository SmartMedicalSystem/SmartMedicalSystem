namespace Application.DTOs.PatientResultElement
{
    public class PatientResultElementCreateDto
    {
        public int PatientResultId { get; set; }
        public int ElementId { get; set; }
        public string Value { get; set; } = null!;
        public string? Comment { get; set; }
        public int TechId { get; set; }
    }
}
