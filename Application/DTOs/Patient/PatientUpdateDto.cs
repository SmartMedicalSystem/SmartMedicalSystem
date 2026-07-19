using Domain.Enums;

namespace Application.DTOs.Patient
{
    public class PatientUpdateDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int NationalId { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public int MobileNumber { get; set; }

        public string Address { get; set; } = string.Empty;

        public BloodType BloodType { get; set; }
    }
}
