using Domain.Common;

namespace Domain.Entities
{
    public class LabTestElement
    {
        public int LabTestId { get;  set; }
        public LabTest LabTest { get; set; } = null!;

        public int ElementId { get;  set; }
        public Element Element { get; set; } = null!;

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }

        private LabTestElement() { }

        public LabTestElement(int labTestId, int elementId, int displayOrder = 0, bool isRequired = true)
        {
            LabTestId = Guard.Positive(labTestId, nameof(labTestId));
            ElementId = Guard.Positive(elementId, nameof(elementId));
            if (displayOrder < 0)
                throw new System.ArgumentException($"{nameof(displayOrder)} cannot be negative.", nameof(displayOrder));

            DisplayOrder = displayOrder;
            IsRequired = isRequired;
        }
    }
}
