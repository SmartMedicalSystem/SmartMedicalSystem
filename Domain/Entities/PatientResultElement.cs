using Domain.Common;

namespace Domain.Entities
{
    public class PatientResultElement : BaseEntity
    {
        public int PatientResultId { get;  set; }

        public int ElementId { get;  set; }

        public string Value { get;  set; } = null!;

        public string? Comment { get; set; }

        public int TechId { get;  set; }

        // Navigation Properties
        public PatientResult patientResult { get; set; } = null!; // naming kept exactly as in the source entity

        public Element Element { get; set; } = null!;

        public LabTechnician Technician { get; set; } = null!;

        private PatientResultElement() { }

        public PatientResultElement(int patientResultId, int elementId, string value, int techId, string? comment = null)
        {
            PatientResultId = Guard.Positive(patientResultId, nameof(patientResultId));
            ElementId = Guard.Positive(elementId, nameof(elementId));
            Value = Guard.NotNullOrWhiteSpace(value, nameof(value), 200);
            TechId = Guard.Positive(techId, nameof(techId));
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        }

        public void UpdateValue(string value, string? comment = null)
        {
            Value = Guard.NotNullOrWhiteSpace(value, nameof(value), 200);
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        }
    }
}
