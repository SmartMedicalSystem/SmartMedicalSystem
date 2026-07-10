using Domain.Enums;

namespace Application.DTOs.Doctor
{
    public class DoctorReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public string Contact { get; set; } = null!;
        public Gender Gender { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
