using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get;  set; } = null!;

        public string DepartmentMangager { get;  set; } = null!; // spelling kept exactly as in the source entity

        public int? DoctorId { get;  set; }
        public Doctor? Doctor { get; set; }

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

        private Department() { }

        public Department(string name, string departmentManager)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 100);
            DepartmentMangager = Guard.NotNullOrWhiteSpace(departmentManager, nameof(departmentManager), 100);
        }

        public void UpdateDetails(string name, string departmentManager)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 100);
            DepartmentMangager = Guard.NotNullOrWhiteSpace(departmentManager, nameof(departmentManager), 100);
        }

        /// <summary>Assigns (or reassigns) the head doctor of the department.</summary>
        public void AssignHeadDoctor(int doctorId)
        {
            DoctorId = Guard.Positive(doctorId, nameof(doctorId));
        }

        public void RemoveHeadDoctor()
        {
            DoctorId = null;
        }
    }
}
