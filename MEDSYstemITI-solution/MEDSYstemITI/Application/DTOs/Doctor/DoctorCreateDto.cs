using Domain.Enums;

namespace Application.DTOs.Doctor
{
    public class DoctorCreateDto
    {
        public string Name { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public string Contact { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; } = string.Empty;

        public int MobileNumber { get; set; }

        public string Address { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        public string NationalId { get; set; }

        public int DepartmentId { get; set; }

    }
}
