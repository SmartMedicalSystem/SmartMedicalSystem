using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class TestElement : BaseEntity
    {
        public string ElementName { get;  set; } = null!;

        public string Unit { get;  set; } = null!;

        public float NormalMin { get;  set; }

        public float NormalMax { get;  set; }

        public ICollection<LabTestElement> LabTestElements { get; } = new List<LabTestElement>();

        private TestElement() { }

        public TestElement(string elementName, string unit, float normalMin, float normalMax)
        {
            ElementName = Guard.NotNullOrWhiteSpace(elementName, nameof(elementName), 150);
            Unit = Guard.NotNullOrWhiteSpace(unit, nameof(unit), 30);
            Guard.Range(normalMin, normalMax, nameof(normalMin), nameof(normalMax));
            NormalMin = normalMin;
            NormalMax = normalMax;
        }

        public void UpdateRange(float normalMin, float normalMax)
        {
            Guard.Range(normalMin, normalMax, nameof(normalMin), nameof(normalMax));
            NormalMin = normalMin;
            NormalMax = normalMax;
        }
    }
}
