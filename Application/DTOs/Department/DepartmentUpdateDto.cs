namespace Application.DTOs.Department
{
    public class DepartmentUpdateDto
    {
        public string Name { get; set; } = null!;
        public int? FloorNumber { get; set; }
        public string HeadDoctor { get; set; } = null!;
        public int? HeadDoctorId { get; set; }
        public string? Status { get; set; }
    }
}
