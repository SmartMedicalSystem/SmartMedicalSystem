using Domain.Common;
using Domain.Entities.Baseperson;
using Domain.Enums;

namespace Domain.Entities
{
    public class LabTechnician : BasePerson
    {
        // EF materialization constructor
        public LabTechnician() { }

        public LabTechnician(string name, string contact)
        {
            var cleanName = Guard.NotNullOrWhiteSpace(name, nameof(name), 200);
            var parts = cleanName.Split(' ', 2);
            FirstName = parts[0];
            LastName = parts.Length > 1 ? parts[1] : string.Empty;
            PhoneNumber = Guard.NotNullOrWhiteSpace(contact, nameof(contact), 50);
        }

   
        // Compatibility fields used by services
        public string Username { get; set; } = string.Empty;

        // Personal Information
        // Employment / lab-specific fields
        public string Laboratory { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public EmploymentStatus EmploymentStatus { get; set; }
        public WorkShift WorkShift { get; set; }
        public DateOnly JoiningDate { get; set; }
        public int YearsOfExperience { get; set; }

        // Staff/account fields specific to technicians
        //public string employeeidentitynumber { get; set; } = string.Empty;
    }
}
