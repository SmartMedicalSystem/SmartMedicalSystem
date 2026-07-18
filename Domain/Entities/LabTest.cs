using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class LabTest : BaseEntity
    {
        public string TestName { get;  set; } = null!;

        public string Description { get;  set; } = null!;
        public ICollection<LabTestElement> LabTestElements { get; } = new List<LabTestElement>();

        private LabTest() { }

        public LabTest(string testName, string description)
        {
            TestName = Guard.NotNullOrWhiteSpace(testName, nameof(testName), 150);
            Description = Guard.NotNullOrWhiteSpace(description, nameof(description), 500);
        }

        public void UpdateDetails(string testName, string description)
        {
            TestName = Guard.NotNullOrWhiteSpace(testName, nameof(testName), 150);
            Description = Guard.NotNullOrWhiteSpace(description, nameof(description), 500);
        }
    }
}
