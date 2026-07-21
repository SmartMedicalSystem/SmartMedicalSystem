using System;
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Full profile record for a Lab Technician (self-service "My Profile" page).
    /// Split into 3 sections matching the UI: Personal / Professional / Contact info.
    /// Personal + Contact are editable by the technician themselves.
    /// Professional info (EmployeeId, AssignedLaboratory, JobTitle, YearsOfExperience)
    /// and Status are admin-only fields (shown read-only/gray on the tech's own page).
    /// </summary>
    public class LabTechProfile : BaseEntity
    {
        /// <summary>FK -> ApplicationUser.Id, links this profile to the login account.</summary>
        public int UserId { get; private set; }

        // ----- Personal Information -----
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public Gender Gender { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Nationality { get; private set; } = null!;
        public string NationalId { get; private set; } = null!;
        public string? ProfilePictureUrl { get; private set; }

        // ----- Professional Information (admin-managed) -----
        public string EmployeeId { get; private set; } = null!;
        public string AssignedLaboratory { get; private set; } = null!;
        public string JobTitle { get; private set; } = null!;
        public int YearsOfExperience { get; private set; }
        public ProfileStatus Status { get; private set; }
        public DateTime JoiningDate { get; private set; }

        // ----- Contact Information -----
        public string Email { get; private set; } = null!;
        public string PhoneNumber { get; private set; } = null!;
        public string Address { get; private set; } = null!;

        private LabTechProfile() { }

        public LabTechProfile(
            int userId,
            string firstName,
            string lastName,
            Gender gender,
            DateTime dateOfBirth,
            string nationality,
            string nationalId,
            string employeeId,
            string assignedLaboratory,
            string jobTitle,
            int yearsOfExperience,
            DateTime joiningDate,
            string email,
            string phoneNumber,
            string address)
        {
            UserId = Guard.Positive(userId, nameof(userId));
            FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName), 100);
            LastName = Guard.NotNullOrWhiteSpace(lastName, nameof(lastName), 100);
            Gender = gender;
            DateOfBirth = Guard.NotInFuture(Guard.NotDefault(dateOfBirth, nameof(dateOfBirth)), nameof(dateOfBirth));
            Nationality = Guard.NotNullOrWhiteSpace(nationality, nameof(nationality), 100);
            NationalId = Guard.NotNullOrWhiteSpace(nationalId, nameof(nationalId), 50);

            EmployeeId = Guard.NotNullOrWhiteSpace(employeeId, nameof(employeeId), 50);
            AssignedLaboratory = Guard.NotNullOrWhiteSpace(assignedLaboratory, nameof(assignedLaboratory), 150);
            JobTitle = Guard.NotNullOrWhiteSpace(jobTitle, nameof(jobTitle), 100);
            YearsOfExperience = yearsOfExperience;
            JoiningDate = Guard.NotDefault(joiningDate, nameof(joiningDate));
            Status = ProfileStatus.Active;

            Email = Guard.NotNullOrWhiteSpace(email, nameof(email), 150);
            PhoneNumber = Guard.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber), 30);
            Address = Guard.NotNullOrWhiteSpace(address, nameof(address), 250);
        }

        /// <summary>Self-service: technician updates their own Personal Information section.</summary>
        public void UpdatePersonalInfo(string firstName, string lastName, Gender gender, DateTime dateOfBirth, string nationality, string nationalId)
        {
            FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName), 100);
            LastName = Guard.NotNullOrWhiteSpace(lastName, nameof(lastName), 100);
            Gender = gender;
            DateOfBirth = Guard.NotInFuture(Guard.NotDefault(dateOfBirth, nameof(dateOfBirth)), nameof(dateOfBirth));
            Nationality = Guard.NotNullOrWhiteSpace(nationality, nameof(nationality), 100);
            NationalId = Guard.NotNullOrWhiteSpace(nationalId, nameof(nationalId), 50);
        }

        /// <summary>Self-service: technician updates their own Contact Information section.</summary>
        public void UpdateContactInfo(string email, string phoneNumber, string address)
        {
            Email = Guard.NotNullOrWhiteSpace(email, nameof(email), 150);
            PhoneNumber = Guard.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber), 30);
            Address = Guard.NotNullOrWhiteSpace(address, nameof(address), 250);
        }

        /// <summary>Self-service: updates the avatar shown top-left (camera icon in the UI).</summary>
        public void UpdateProfilePicture(string? profilePictureUrl)
        {
            ProfilePictureUrl = string.IsNullOrWhiteSpace(profilePictureUrl) ? null : profilePictureUrl.Trim();
        }

        /// <summary>Admin-only: updates the Professional Information section.</summary>
        public void UpdateProfessionalInfo(string assignedLaboratory, string jobTitle, int yearsOfExperience)
        {
            AssignedLaboratory = Guard.NotNullOrWhiteSpace(assignedLaboratory, nameof(assignedLaboratory), 150);
            JobTitle = Guard.NotNullOrWhiteSpace(jobTitle, nameof(jobTitle), 100);
            YearsOfExperience = yearsOfExperience;
        }

        /// <summary>Admin-only: activates / deactivates / sets on-leave for the technician.</summary>
        public void UpdateStatus(ProfileStatus status)
        {
            Status = status;
        }
    }
}
