using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Laboratory : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public string Location { get; private set; } = null!;
        public string Phone { get; private set; } = null!;
        public LabStatus Status { get; private set; }

        public string? Code { get; private set; }           
        public string? Specialty { get; private set; }       
     

        public int? HeadTechnicianId { get; private set; }
        public int? DepartmentId { get; private set; }

        // Navigation Properties
        public LabTechnician? HeadTechnician { get; set; }
        public Department? Department { get; set; }
        public ICollection<LabTest> LabTests { get; private set; } = new List<LabTest>();
        public ICollection<LabTechnician> LabTechnicians { get; private set; } = new List<LabTechnician>();

        private Laboratory() { }

        public Laboratory(
            string name,
            string location,
            string phone,
            LabStatus status = LabStatus.Active,
            int? headTechnicianId = null,
            int? departmentId = null,
            string? code = null,
            string? specialty = null)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 100);
            Location = Guard.NotNullOrWhiteSpace(location, nameof(location), 200);
            Phone = Guard.ValidatePhone(phone);
            Status = status;
            HeadTechnicianId = headTechnicianId;
            DepartmentId = departmentId;
            Code = code;
            Specialty = specialty;
        }

        public void UpdateDetails(
            string name,
            string location,
            string phone,
            LabStatus? status = null,
            int? headTechnicianId = null,
            int? departmentId = null,
            string? code = null,
            string? specialty = null)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 100);
            Location = Guard.NotNullOrWhiteSpace(location, nameof(location), 200);
            Phone = Guard.ValidatePhone(phone);

            if (status.HasValue)
                Status = status.Value;

            HeadTechnicianId = headTechnicianId;
            DepartmentId = departmentId;

            if (!string.IsNullOrWhiteSpace(code))
                Code = code;

            if (!string.IsNullOrWhiteSpace(specialty))
                Specialty = specialty;
        }

        public void AssignHeadTechnician(int technicianId)
        {
            HeadTechnicianId = Guard.Positive(technicianId, nameof(technicianId));
        }

        public void RemoveHeadTechnician()
        {
            HeadTechnicianId = null;
        }

        public void AddTechnician(LabTechnician technician)
        {
            if (technician == null) throw new ArgumentNullException(nameof(technician));
            if (!LabTechnicians.Contains(technician))
                LabTechnicians.Add(technician);
        }

        public void RemoveTechnician(LabTechnician technician)
        {
            if (technician == null) throw new ArgumentNullException(nameof(technician));
            if (LabTechnicians.Contains(technician))
                LabTechnicians.Remove(technician);
        }
    }
}