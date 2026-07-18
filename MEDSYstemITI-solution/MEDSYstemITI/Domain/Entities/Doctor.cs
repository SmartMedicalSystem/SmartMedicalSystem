using Domain.Common;
using Domain.Enums;
using Domain.Entities.Baseperson;

namespace Domain.Entities
{
    public class Doctor : BasePerson
    {
        // National identifier (string allows leading zeros or mixed formats)
        public string Specialization { get; private set; } = string.Empty;

        public int DepartmentId { get; private set; }   // FK -> Department (required staff membership)

        public Department Department { get; private set; } = null!;

        // EF Core materialization constructor
        protected Doctor() { }

      

        // Constructor that accepts first & last name explicitly
        public Doctor(
            string firstName,
            string lastName,
            string specialization,
            string phoneNumber,
            Gender gender,
            int departmentId)
            : base(firstName, lastName, DateTime.Today) // use a non-default DOB for materialization; real DOB may be set later
        {
            Specialization = Guard.NotNullOrWhiteSpace(specialization, nameof(specialization), 100);
            PhoneNumber = Guard.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber), 50);
            Gender = gender;
            DepartmentId = Guard.Positive(departmentId, nameof(departmentId));
        }

        // Overload matching service DTOs (includes contact and int mobile number)
        public void UpdateProfile(string name, string specialization, string contact, Gender gender, string email, int mobileNumber, string address)
        {
            var cleanName = Guard.NotNullOrWhiteSpace(name, nameof(name), 200);
            var parts = cleanName.Split(' ', 2);
            FirstName = parts[0];
            LastName = parts.Length > 1 ? parts[1] : string.Empty;

            Specialization = Guard.NotNullOrWhiteSpace(specialization, nameof(specialization), 100);
            // prefer explicit contact if provided, otherwise use mobileNumber
            if (!string.IsNullOrWhiteSpace(contact))
                PhoneNumber = Guard.NotNullOrWhiteSpace(contact, nameof(contact), 50);
            else
                PhoneNumber = Guard.NotNullOrWhiteSpace(mobileNumber.ToString(), nameof(mobileNumber), 50);

            Gender = gender;
            Email = Guard.NotNullOrWhiteSpace(email, nameof(email), 150);
            Address = Guard.NotNullOrWhiteSpace(address, nameof(address), 250);
        }

        // Compatibility constructor used by services/seeds that provide a single full name string
        public Doctor(string fullName, string specialization, string phoneNumber, Gender gender, int departmentId)
            : base(
                  Guard.NotNullOrWhiteSpace(fullName, nameof(fullName), 200).Split(' ', 2).First(),
                  Guard.NotNullOrWhiteSpace(fullName, nameof(fullName), 200).Split(' ', 2).ElementAtOrDefault(1) ?? string.Empty,
                  DateTime.Today)
        {
            Specialization = Guard.NotNullOrWhiteSpace(specialization, nameof(specialization), 100);
            PhoneNumber = Guard.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber), 50);
            Gender = gender;
            DepartmentId = Guard.Positive(departmentId, nameof(departmentId));
        }

        // Extended profile update to include contact details and address (mobile handled via BasePerson)

        /// <summary>Updates the mutable profile fields of the doctor, re-validating each one.</summary>
        public void UpdateProfile(string name, string specialization, Gender gender,
            string email, string mobileNumber, string address)
        {
            var cleanName = Guard.NotNullOrWhiteSpace(name, nameof(name), 200);
            var parts = cleanName.Split(' ', 2);
            FirstName = parts[0];
            LastName = parts.Length > 1 ? parts[1] : string.Empty;

            Specialization = Guard.NotNullOrWhiteSpace(specialization, nameof(specialization), 100);
            PhoneNumber = Guard.NotNullOrWhiteSpace(mobileNumber, nameof(mobileNumber), 50);
            Gender = gender;
            Email = Guard.NotNullOrWhiteSpace(email, nameof(email), 100);
            Address = Guard.NotNullOrWhiteSpace(address, nameof(address), 250);
        }

        /// <summary>Moves the doctor to a different department.</summary>
        public void ReassignDepartment(int departmentId)
        {
            DepartmentId = Guard.Positive(departmentId, nameof(departmentId));
        }
    }
}
