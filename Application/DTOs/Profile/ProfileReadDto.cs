using Domain.Enums;
using System;

namespace Application.DTOs.Profile
{
    public class ProfileReadDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // Personal Information
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        public string? ProfilePictureUrl { get; set; }

        // Professional Information
        public string EmployeeId { get; set; } = null!;
        public string AssignedLaboratory { get; set; } = null!;
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