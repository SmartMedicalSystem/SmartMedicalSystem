using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class Element : BaseEntity
    {
        public string Name { get; set; } = null!;

        public string? Unit { get; set; }

        public string? ReferenceRange { get; set; }

        public string? Description { get; set; }

        public ICollection<LabTestElement> LabTestElements { get; set; } = new List<LabTestElement>();

        public ICollection<PatientResultElement> PatientResultElements { get; set; } = new List<PatientResultElement>();

        private Element() { }

        public Element(string name, string? unit = null, string? referenceRange = null, string? description = null)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 150);
            Unit = string.IsNullOrWhiteSpace(unit) ? null : unit.Trim();
            ReferenceRange = string.IsNullOrWhiteSpace(referenceRange) ? null : referenceRange.Trim();
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        public void UpdateDetails(string name, string? unit, string? referenceRange, string? description)
        {
            Name = Guard.NotNullOrWhiteSpace(name, nameof(name), 150);
            Unit = string.IsNullOrWhiteSpace(unit) ? null : unit.Trim();
            ReferenceRange = string.IsNullOrWhiteSpace(referenceRange) ? null : referenceRange.Trim();
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }
    }
}
