using Domain.Enums;

namespace Application.DTOs.LabTechnician
{
    public class LabTechnicianReadDto
    {
        public int Id { get; set; }
        // Personal Information

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public Gender Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Nationality { get; set; } = null!;

        public string NationalId { get; set; } = null!;

        // Employment

        public string EmployeeId { get; set; } = null!;

        public string Laboratory { get; set; } = null!;

        public string JobTitle { get; set; } = null!;

        public EmploymentStatus EmploymentStatus { get; set; }

        public WorkShift WorkShift { get; set; }

        public DateOnly JoiningDate { get; set; }

        public int YearsOfExperience { get; set; }

        // Contact Information

        public string PhoneNumber { get; set; } = null!;

        public string? AlternativePhone { get; set; }

        public string Email { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Country { get; set; } = null!;

        public string? PostalCode { get; set; }

        // Account Information

        public string Username { get; set; } = null!;

        public bool AllowLogin { get; set; }

        public bool AccountActive { get; set; }

        public bool ReceiveNotifications { get; set; }

        public bool SendWelcomeEmail { get; set; }

        public bool SendLoginCredentials { get; set; }

        public string? PhotoUrl { get; set; }
    }
}
