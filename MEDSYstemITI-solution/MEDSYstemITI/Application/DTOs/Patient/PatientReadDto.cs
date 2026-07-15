using Domain.Enums;

namespace Application.DTOs.Patient
{
    public class PatientReadDto
    {
        public int Id { get; set; }
           
        public int NationalId { get; set; }
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int Age { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public int MobileNumber { get; set; }

        public string Address { get; set; } = string.Empty;

        public BloodType BloodType { get; set; }
    }
}
