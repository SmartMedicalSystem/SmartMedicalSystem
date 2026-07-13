using Domain.Common;
using Domain.Enums;
using Domain.Entities.Baseperson;

namespace Domain.Entities
{
    public class Doctor : BasePerson
    {
        public string Name { get;  set; } = null!;

        public string Specialization { get;  set; } = null!;

        public string Contact { get;  set; } = null!;

        public DateTime DateOfBirth { get;  set; }

        public string Email { get; set; } = null!;
        public int MobileNumber { get; set; }
        public string Address { get; set; } = null!;
        public Gender Gender { get; set; }

        public int DepartmentId { get; private set; }   // FK -> Department (required staff membership)

        public Department Department { get; private set; } = null!;

        // EF Core materialization constructor (EF Core can bind to a non-public
        // parameterless constructor; application code must go through the
        // validated constructor below instead).
        private Doctor() { }

        public Doctor(string name, string specialization, string contact, Gender gender, int departmentId)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 100);
            Specialization = Guard.NotNullOrWhiteSpace(specialization, nameof(specialization), 100);
            Contact = Guard.NotNullOrWhiteSpace(contact, nameof(contact), 50);
            Gender = gender;
            DepartmentId = Guard.Positive(departmentId, nameof(departmentId));
        }

        /// <summary>Updates the mutable profile fields of the doctor, re-validating each one.</summary>
        public void UpdateProfile(string name, string specialization, string contact, Gender gender, string email, int mobileNumber, string address)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 100);
            Specialization = Guard.NotNullOrWhiteSpace(specialization, nameof(specialization), 100);
            Contact = Guard.NotNullOrWhiteSpace(contact, nameof(contact), 50);
            Gender = gender;
            Email = Guard.NotNullOrWhiteSpace(email, nameof(email), 100);
            MobileNumber = mobileNumber;
            Address = Guard.NotNullOrWhiteSpace(address, nameof(address), 250);
        }

        /// <summary>Moves the doctor to a different department.</summary>
        public void ReassignDepartment(int departmentId)
        {
            DepartmentId = Guard.Positive(departmentId, nameof(departmentId));
        }
    }
}
