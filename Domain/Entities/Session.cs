using System;
using Domain.Common;

namespace Domain.Entities
{
    public class Session : BaseEntity
    {
        public int PatientId { get;  set; }

        public int DoctorId { get;  set; }

        public int DeptId { get;  set; }

        public DateTime SessionDate { get;  set; }

        public string? Notes { get;  set; }

        // Navigation Properties
        public Patient Patient { get; set; } = null!;

        public Doctor Doctor { get; set; } = null!;

        public Department Department { get; set; } = null!;

        private Session() { }

        public Session(int patientId, int doctorId, int deptId, DateTime sessionDate, string? notes = null)
        {
            PatientId = Guard.Positive(patientId, nameof(patientId));
            DoctorId = Guard.Positive(doctorId, nameof(doctorId));
            DeptId = Guard.Positive(deptId, nameof(deptId));
            SessionDate = Guard.NotDefault(sessionDate, nameof(sessionDate));
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        }

        public void Reschedule(DateTime sessionDate)
        {
            SessionDate = Guard.NotDefault(sessionDate, nameof(sessionDate));
        }

        public void UpdateNotes(string? notes)
        {
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        }
    }
}
