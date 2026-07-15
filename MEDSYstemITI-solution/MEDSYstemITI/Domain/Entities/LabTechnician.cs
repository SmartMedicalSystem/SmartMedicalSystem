using Domain.Common;
using Domain.Entities.Baseperson;
using Domain.Enums;

namespace Domain.Entities
{
    public class LabTechnician : BasePerson
    {
        // Personal Information
        // Employment / lab-specific fields
        public string Laboratory { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public EmploymentStatus EmploymentStatus { get; set; }
        public WorkShift WorkShift { get; set; }
        public DateOnly JoiningDate { get; set; }
        public int YearsOfExperience { get; set; }

        // Staff/account fields specific to technicians
        public string EmployeeId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
