
using Domain.Common;
using Domain.Enums;
using Domain.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Baseperson
{
    // Base class for all person-like entities (Patient, Doctor, LabTechnician)
    public abstract class BasePerson : BaseEntity
    {
        // Personal identifiers
        public string EncryptedNationalId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public Gender Gender { get; set; }

        // Use DateTime for consistency with other DTOs and services
        public DateTime DateOfBirth { get; set; }
        [NotMapped]
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;

                if (DateOfBirth.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }


        // National identity / profile
        public string Nationality { get; set; } = string.Empty;

        // Common contact / account profile fields
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AlternativePhone { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? PostalCode { get; set; }

        // Backwards-compatible single-field name used by some parts of the app/migrations.
        // Keep it synchronized with FirstName/LastName.
        public string Name
        {
            get => FullName;
            set
            {
                var clean = Guard.NotNullOrWhiteSpace(value, nameof(Name), 200);
                var parts = clean.Split(' ', 2);
                FirstName = parts[0];
                LastName = parts.Length > 1 ? parts[1] : string.Empty;
            }
        }

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
        }

      
        public void UpdateProfile(string firstName, string lastName, DateTime dateOfBirth)
        {
            FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName), 100);
            LastName = Guard.NotNullOrWhiteSpace(lastName, nameof(lastName), 100);
            DateOfBirth = Guard.NotInFuture(Guard.NotDefault(dateOfBirth, nameof(dateOfBirth)), nameof(dateOfBirth));
        }
    }
}
