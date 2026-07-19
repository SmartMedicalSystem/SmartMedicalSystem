using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.LabTechnician
{
    public class LabTechnicianCreateDto
    {
        // Personal Information

<<<<<<< Updated upstream
        public string FirstName { get; set; } 

        public string LastName { get; set; } 
=======
        public string FirstName { get; set; }

        public string LastName { get; set; }
>>>>>>> Stashed changes
        public Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

<<<<<<< Updated upstream
        public string Nationality { get; set; } 
=======
        public string Nationality { get; set; }
>>>>>>> Stashed changes

        public string NationalId { get; set; }
        // Employment

<<<<<<< Updated upstream
        public string Laboratory { get; set; } 
        public string JobTitle { get; set; } 
=======
        public string Laboratory { get; set; }
        public string JobTitle { get; set; }
>>>>>>> Stashed changes

        public EmploymentStatus EmploymentStatus { get; set; }

        public WorkShift WorkShift { get; set; }

        public DateOnly JoiningDate { get; set; }

        public int YearsOfExperience { get; set; }

        // Contact Information

<<<<<<< Updated upstream
        public string PhoneNumber { get; set; } 
=======
        public string PhoneNumber { get; set; }
>>>>>>> Stashed changes

        public string? AlternativePhone { get; set; }

        public string Email { get; set; }
        public string Address { get; set; }

<<<<<<< Updated upstream
        public string City { get; set; } 
        public string Country { get; set; } 
=======
        public string City { get; set; }
        public string Country { get; set; }
>>>>>>> Stashed changes

        public string? PostalCode { get; set; }

        // Account Information

<<<<<<< Updated upstream
        public string Username { get; set; } 

        public string Password { get; set; } 
=======
        public string Username { get; set; }

        public string Password { get; set; }
>>>>>>> Stashed changes

        public bool AllowLogin { get; set; }

        public bool AccountActive { get; set; }

        public bool ReceiveNotifications { get; set; }

        public bool SendWelcomeEmail { get; set; }

        public bool SendLoginCredentials { get; set; }

        public IFormFile? PhotoUrl { get; set; }
    }
}
