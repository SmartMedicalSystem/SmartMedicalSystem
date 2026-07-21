namespace Application.DTOs.PatientResultElement
{
    public class PatientResultElementReadDto
    {
        public int Id { get; set; }
        public int PatientResultId { get; set; }
        public int ElementId { get; set; }
        public string ElementName { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string? Comment { get; set; }
        public int TechId { get; set; }
    }
}
