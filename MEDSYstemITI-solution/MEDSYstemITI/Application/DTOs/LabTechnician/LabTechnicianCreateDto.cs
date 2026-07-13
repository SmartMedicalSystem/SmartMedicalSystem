namespace Application.DTOs.LabTechnician
{
    public class LabTechnicianCreateDto
    {
        public string Name { get; set; } = null!;
        public string Contact { get; set; } = null!;
        public int NationalId { get; set; }
    }
}
