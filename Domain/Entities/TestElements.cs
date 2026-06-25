using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class TestElements: BaseEntity
    {
        public int TestId { get; private set; }
        public string ElementName { get; private set; }
        public string Unit { get; private set; }
        public float NormalMin { get; private set; }
        public float NormalMax { get; private set; }

        public LabTest LabTest { get; private set; }

        private TestElement() { }

        public TestElement(int testId, string elementName, string unit, float normalMin, float normalMax)
        {
            if (testId <= 0)
                throw new ArgumentException("TestId is unavailable");

            if (string.IsNullOrWhiteSpace(elementName))
                throw new ArgumentException("The name of the required elemnt ");

            if (normalMin > normalMax)
                throw new ArgumentException("The minimum value cannot be greater than the maximum value");

            TestId = testId;
            ElementName = elementName;
            Unit = unit;
            NormalMin = normalMin;
            NormalMax = normalMax;
        }

        public bool IsValueNormal(float value)
        {
            return value >= NormalMin && value <= NormalMax;
        }

        public void UpdateDetails(string elementName, string unit, float normalMin, float normalMax)
        {
            if (string.IsNullOrWhiteSpace(elementName))
                throw new ArgumentException("The name of the required element");

            if (normalMin > normalMax)
                throw new ArgumentException("The minimum value cannot be greater than the maximum value");

            ElementName = elementName;
            Unit = unit;
            NormalMin = normalMin;
            NormalMax = normalMax;
        }
    }
}
