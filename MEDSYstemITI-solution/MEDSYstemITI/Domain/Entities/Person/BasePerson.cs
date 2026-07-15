
using Domain.Common;
using Domain.Enums;
using Domain.Identity;

namespace Domain.Entities.Baseperson
{
    // Base class for all person-like entities (Patient, Doctor, LabTechnician)
    public abstract class BasePerson : BaseEntity
    {
        // Personal identifiers
        public string EncryptedSSN { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Gender Gender { get; set; }

        // Use DateTime for consistency with other DTOs and services
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; } = 0;

        // National identity / profile
        public string Nationality { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;

        // Common contact / account profile fields
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AlternativePhone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? PostalCode { get; set; }

        public bool AllowLogin { get; set; }
        public bool AccountActive { get; set; }
        public bool ReceiveNotifications { get; set; }
        public string? PhotoUrl { get; set; }

        // Link to Identity user (optional)
        public int? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        // EF materialization constructor
        protected BasePerson() { }

        public BasePerson(string firstName, string lastName, DateTime dateOfBirth)
        {
            FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName), 100);
            LastName = Guard.NotNullOrWhiteSpace(lastName, nameof(lastName), 100);
            DateOfBirth = Guard.NotInFuture(Guard.NotDefault(dateOfBirth, nameof(dateOfBirth)), nameof(dateOfBirth));
            Age = CalculateAge(DateOfBirth);
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.UtcNow.Date;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }

        public void UpdateProfile(string firstName, string lastName, DateTime dateOfBirth)
        {
            FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName), 100);
            LastName = Guard.NotNullOrWhiteSpace(lastName, nameof(lastName), 100);
            DateOfBirth = Guard.NotInFuture(Guard.NotDefault(dateOfBirth, nameof(dateOfBirth)), nameof(dateOfBirth));
            Age = CalculateAge(DateOfBirth);
        }
    }
}
