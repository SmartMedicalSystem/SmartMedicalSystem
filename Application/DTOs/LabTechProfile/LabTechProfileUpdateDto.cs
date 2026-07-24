using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;

namespace Application.DTOs.LabTechProfile
{
    /// <summary>
    /// Self-service update: what the technician can edit on their own page —
    /// Personal Information + Contact Information sections (Save Changes button).
    /// </summary>
    public class LabTechProfileUpdateDto
    {
        // Personal Information

        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;

        // Contact Information
        public string? Email { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public string? Address { get; set; } = null!;

        public IFormFile? PhotoUrl { get; set; }


    }
}
