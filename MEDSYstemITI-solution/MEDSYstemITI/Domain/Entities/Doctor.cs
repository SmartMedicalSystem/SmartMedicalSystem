using Domain.Common;
using Domain.Enums;
using Domain.Entities.Baseperson;

namespace Domain.Entities
{
    public class Doctor : BasePerson
    {
        public string Specialization { get; private set; } = string.Empty;

        // Keep a Name and Contact property for compatibility with existing configs/queries
        public string Name { get; private set; } = string.Empty;
        public string Contact { get; private set; } = string.Empty;

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
            Contact = PhoneNumber;
            Gender = gender;
            DepartmentId = Guard.Positive(departmentId, nameof(departmentId));
        }

        /// <summary>Updates the mutable profile fields of the doctor, re-validating each one.</summary>
        public void UpdateProfile(string name, string specialization, string contact, Gender gender)
        {
            var cleanName = Guard.NotNullOrWhiteSpace(name, nameof(name), 200);
            var parts = cleanName.Split(' ', 2);
            FirstName = parts[0];
            LastName = parts.Length > 1 ? parts[1] : string.Empty;
            Name = cleanName;

            Specialization = Guard.NotNullOrWhiteSpace(specialization, nameof(specialization), 100);
            PhoneNumber = Guard.NotNullOrWhiteSpace(contact, nameof(contact), 50);
            Contact = PhoneNumber;
            Gender = gender;
        }

        /// <summary>Moves the doctor to a different department.</summary>
        public void ReassignDepartment(int departmentId)
        {
            DepartmentId = Guard.Positive(departmentId, nameof(departmentId));
        }
    }
}
