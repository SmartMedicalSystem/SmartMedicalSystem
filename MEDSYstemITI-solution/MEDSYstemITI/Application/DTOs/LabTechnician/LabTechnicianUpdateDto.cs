using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.LabTechnician
{
    public class LabTechnicianUpdateDto
    {
        // Personal Information
        public string FirstName { get; set; } 

        public string LastName { get; set; } 

        public Gender Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Nationality { get; set; } 

        public string NationalId { get; set; } 

        // Employment Information
        public string Laboratory { get; set; } 

        public string JobTitle { get; set; } 

        public EmploymentStatus EmploymentStatus { get; set; }

        public WorkShift WorkShift { get; set; }

        public DateOnly JoiningDate { get; set; }

        public int YearsOfExperience { get; set; }

        // Contact Information
        public string PhoneNumber { get; set; } 

        public string? AlternativePhone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; } 

        public string City { get; set; } 

        public string Country { get; set; } 

        public string? PostalCode { get; set; }

        // Account Information
        public string Username { get; set; } 

        public bool AllowLogin { get; set; }

        public bool AccountActive { get; set; }

        public bool ReceiveNotifications { get; set; }

        public IFormFile? PhotoUrl { get; set; }
    }
}

