using Domain.Common;

namespace Domain.Entities
{
    public class LabTestElement
    {
        public int LabTestId { get;  set; }
        public LabTest LabTest { get; set; } = null!;

        public int TestElementId { get;  set; }
        public TestElement TestElement { get; set; } = null!;

        private LabTestElement() { }

        public LabTestElement(int labTestId, int testElementId)
        {
            LabTestId = Guard.Positive(labTestId, nameof(labTestId));
            TestElementId = Guard.Positive(testElementId, nameof(testElementId));
        }
    }
}
