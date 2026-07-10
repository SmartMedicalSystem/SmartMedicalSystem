namespace Application.DTOs.PatientResultElement
{
    public class PatientResultElementReadDto
    {
        public int Id { get; set; }
        public int PatientResultId { get; set; }
        public int TestElementId { get; set; }
        public double Value { get; set; }
        public int TechId { get; set; }
    }
}
