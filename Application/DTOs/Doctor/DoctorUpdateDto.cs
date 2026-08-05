using Domain.Enums;

namespace Application.DTOs.Doctor
{
    public class DoctorUpdateDto
    {
        public string Name { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public string Contact { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        public string NationalId { get; set; }

        public int DepartmentId { get; set; }

        public string City { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string? PostalCode { get; set; } 
    }
}
