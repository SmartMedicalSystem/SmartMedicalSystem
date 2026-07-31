using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; private set; } = null!;

        // Head Doctor Name
        public string HeadDoctor { get; private set; } = null!;

        // Floor Number
        public int? FloorNumber { get; private set; }

        // Status (Active / Inactive / Maintenance)
        public string Status { get; private set; } = "Active";

        // Foreign Key to Head Doctor (Doctor entity)
        public int? HeadDoctorId { get; private set; }
        public Doctor? HeadDoctorEntity { get; set; }

        // Collection of Doctors in this department
        public ICollection<Doctor> Doctors { get; private set; } = new List<Doctor>();
        public ICollection<Laboratory> Laboratories { get; set; }
    = new List<Laboratory>();

        // Computed property for Frontend (not stored in DB)
        public int DoctorCount => Doctors?.Count ?? 0;

        private Department() { }

        public Department(
            string name,
            string headDoctor,
            int? floorNumber = null,
          
            string status = "Active")
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 100);
            HeadDoctor = Guard.NotNullOrWhiteSpace(headDoctor, nameof(headDoctor), 100);
            FloorNumber = floorNumber;
           
            Status = Guard.NotNullOrWhiteSpace(status, nameof(status), 50);
        }

        public void UpdateDetails(
            string name,
            string headDoctor,
            int? floorNumber = null,
            string? status = null)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 100);
            HeadDoctor = Guard.NotNullOrWhiteSpace(headDoctor, nameof(headDoctor), 100);
            FloorNumber = floorNumber;
           

            if (!string.IsNullOrWhiteSpace(status))
            {
                Status = status;
            }
        }

        public void AssignHeadDoctor(int doctorId, string doctorName)
        {
            HeadDoctorId = Guard.Positive(doctorId, nameof(doctorId));
            HeadDoctor = Guard.NotNullOrWhiteSpace(doctorName, nameof(doctorName), 100);
        }

        public void RemoveHeadDoctor()
        {
            HeadDoctorId = null;
            HeadDoctor = "Not Assigned";
        }

        public void AddDoctor(Doctor doctor)
        {
            if (doctor == null) throw new ArgumentNullException(nameof(doctor));
            if (!Doctors.Contains(doctor))
            {
                Doctors.Add(doctor);
            }
        }

        public void RemoveDoctor(Doctor doctor)
        {
            if (doctor == null) throw new ArgumentNullException(nameof(doctor));
            if (Doctors.Contains(doctor))
            {
                Doctors.Remove(doctor);
            }
        }
    }
}
