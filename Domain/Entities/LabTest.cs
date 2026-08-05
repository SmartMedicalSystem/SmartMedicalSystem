using System.Collections.Generic;
using Domain.Enums;
using Domain.Common;

namespace Domain.Entities
{
    public class LabTest : BaseEntity
    {
        public string TestName { get; set; } = null!;

        //public LabTestStatus Status { get; set; } = LabTestStatus.Pending;

        public string Description { get; set; } = null!;
        //ahmed realation with laboratory

        public int? LaboratoryId { get; private set; }  // ← NEW: FK to Laboratory
        public Laboratory? Laboratory { get; set; }



        public ICollection<LabTestElement> LabTestElements { get; } = new List<LabTestElement>();

        // Relationship to RequestLabs via explicit join entity
        public ICollection<RequestLabTest> RequestLabTests { get; set; } = new List<RequestLabTest>();

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

        // ===== NEW Methods =====
        public void AssignToLaboratory(int laboratoryId)
        {
            LaboratoryId = Guard.Positive(laboratoryId, nameof(laboratoryId));
        }

        public void RemoveFromLaboratory()
        {
            LaboratoryId = null;
        }


        //public void UpdateStatus(LabTestStatus status)
        //{
        //    Status = status;
        //}

    }
}
