using Application.DTOs.Doctor;

namespace Application.DTOs.Department
{
    public class DepartmentReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? FloorNumber { get; set; }
        public string HeadDoctor { get; set; } = null!;
        public int? HeadDoctorId { get; set; }
        public string Status { get; set; } = null!;
        public int DoctorCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DoctorAtDepartmentDto> Doctors { get; set; } = new();
    }
}
