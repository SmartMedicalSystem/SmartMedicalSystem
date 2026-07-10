using Domain.Enums;

namespace Application.DTOs.Doctor
{
    public class DoctorCreateDto
    {
        public string Name { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public string Contact { get; set; } = null!;
        public Gender Gender { get; set; }
        public int DepartmentId { get; set; }
    }
}
