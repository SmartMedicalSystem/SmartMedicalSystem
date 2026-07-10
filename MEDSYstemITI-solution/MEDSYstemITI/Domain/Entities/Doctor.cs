using Domain.Common;
using Domain.Enums;
using Domain.Entities.Baseperson;

namespace Domain.Entities
{
    public class Doctor : BasePerson
    {
        public string Name { get; private set; } = null!;

        public string Specialization { get; private set; } = null!;

        public string Contact { get; private set; } = null!;

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
        public void UpdateProfile(string name, string specialization, string contact, Gender gender)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 100);
            Specialization = Guard.NotNullOrWhiteSpace(specialization, nameof(specialization), 100);
            Contact = Guard.NotNullOrWhiteSpace(contact, nameof(contact), 50);
            Gender = gender;
        }

        /// <summary>Moves the doctor to a different department.</summary>
        public void ReassignDepartment(int departmentId)
        {
            DepartmentId = Guard.Positive(departmentId, nameof(departmentId));
        }
    }
}
