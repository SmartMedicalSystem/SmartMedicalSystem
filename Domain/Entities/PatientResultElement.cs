using Domain.Common;

namespace Domain.Entities
{
    public class PatientResultElement : BaseEntity
    {
        public int PatientResultId { get;  set; }

        public int TestElementId { get;  set; }

        public double Value { get;  set; }

        public int TechId { get;  set; }

        // Navigation Properties
        public PatientResult patientResult { get; set; } = null!; // naming kept exactly as in the source entity

        public TestElement TestElement { get; set; } = null!;

        public LabTechnician Technician { get; set; } = null!;

        private PatientResultElement() { }

        public PatientResultElement(int patientResultId, int testElementId, double value, int techId)
        {
            PatientResultId = Guard.Positive(patientResultId, nameof(patientResultId));
            TestElementId = Guard.Positive(testElementId, nameof(testElementId));
            Value = Guard.PositiveOrZero(value, nameof(value));
            TechId = Guard.Positive(techId, nameof(techId));
        }

        public void UpdateValue(double value)
        {
            Value = Guard.PositiveOrZero(value, nameof(value));
        }
    }
}
