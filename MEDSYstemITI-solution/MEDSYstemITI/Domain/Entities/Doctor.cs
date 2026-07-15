using Domain.Common;
using Domain.Enums;
using Domain.Entities.Baseperson;

namespace Domain.Entities
{
    public class Doctor : BasePerson
    {
        // National identifier (string allows leading zeros or mixed formats)
        public string NationalId { get; set; } = string.Empty;
        public string Specialization { get; private set; } = string.Empty;

        // Keep a Name and MobileNumber property for compatibility with existing configs/queries
        public string Name { get; private set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public int DepartmentId { get; private set; }   // FK -> Department (required staff membership)

        public Department Department { get; private set; } = null!;

        // EF Core materialization constructor
        protected Doctor() { }

        public Doctor(string name, string specialization, string contact, Gender gender, int departmentId)
        {
            var cleanName = Guard.NotNullOrWhiteSpace(name, nameof(name), 200);
            var parts = cleanName.Split(' ', 2);
            FirstName = parts[0];
            LastName = parts.Length > 1 ? parts[1] : string.Empty;
            Name = cleanName;

            Specialization = Guard.NotNullOrWhiteSpace(specialization, nameof(specialization), 100);
            PhoneNumber = Guard.NotNullOrWhiteSpace(contact, nameof(contact), 50);
            MobileNumber = PhoneNumber;
            Gender = gender;
            DepartmentId = Guard.Positive(departmentId, nameof(departmentId));
        }

        // Extended profile update to include contact details and address
        public void UpdateProfile(string name, string specialization, string contact, Gender gender, string email, int mobileNumber, string address)
        {
            var cleanName = Guard.NotNullOrWhiteSpace(name, nameof(name), 200);
            var parts = cleanName.Split(' ', 2);
            FirstName = parts[0];
            LastName = parts.Length > 1 ? parts[1] : string.Empty;
            Name = cleanName;

            Specialization = Guard.NotNullOrWhiteSpace(specialization, nameof(specialization), 100);
            PhoneNumber = Guard.NotNullOrWhiteSpace(contact, nameof(contact), 50);
            Gender = gender;

            Email = Guard.NotNullOrWhiteSpace(email, nameof(email), 150);
            PhoneNumber = mobileNumber.ToString();
            Address = Guard.NotNullOrWhiteSpace(address, nameof(address), 250);
        }

        /// <summary>Updates the mutable profile fields of the doctor, re-validating each one.</summary>
        public void UpdateProfile(string name, string specialization, Gender gender,
            string email, string mobileNumber, string address)
        {
            var cleanName = Guard.NotNullOrWhiteSpace(name, nameof(name), 200);
            var parts = cleanName.Split(' ', 2);
            FirstName = parts[0];
            LastName = parts.Length > 1 ? parts[1] : string.Empty;
            Name = cleanName;

            Specialization = Guard.NotNullOrWhiteSpace(specialization, nameof(specialization), 100);
            PhoneNumber = Guard.NotNullOrWhiteSpace(mobileNumber, nameof(mobileNumber), 50);
            MobileNumber = PhoneNumber;
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
