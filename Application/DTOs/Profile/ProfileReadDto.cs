using Domain.Enums;

namespace Application.DTOs.Profile
{
    public class ProfileReadDto
    {
        public int Id { get; set; }

        // Personal Information
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; } = null!;
        public string nationalId { get; set; } = null!;
        public string? photoUrl { get; set; }

        // Professional Information
        public int? laboratoryId { get; set; }
        public string? AssignedLaboratory { get; set; }
        public string JobTitle { get; set; } = null!;
        public int YearsOfExperience { get; set; }
        public ProfileStatus Status { get; set; }
        public DateTime JoiningDate { get; set; }

        // Contact Information
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}