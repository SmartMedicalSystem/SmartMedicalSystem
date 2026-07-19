using Domain.Enums;
using System;

namespace Application.DTOs.LabTechProfile
{
    /// <summary>Used by an admin when onboarding a new lab technician.</summary>
    public class LabTechProfileCreateDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; } = null!;
        public string NationalId { get; set; } = null!;

        public string EmployeeId { get; set; } = null!;
        public string AssignedLaboratory { get; set; } = null!;
        public string JobTitle { get; set; } = null!;
        public int YearsOfExperience { get; set; }
        public DateTime JoiningDate { get; set; }

        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
