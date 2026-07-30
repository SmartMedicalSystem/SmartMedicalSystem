using System;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Testing.Domain
{
    public class LabTestTests
    {
        [Fact]
        public void Constructor_Valid_SetsProperties()
        {
            var t = new LabTest("CBC", "Complete blood count");
            t.TestName.Should().Be("CBC");
            t.Description.Should().Be("Complete blood count");
        }

        [Fact]
        public void UpdateDetails_Valid_Updates()
        {
            var t = new LabTest("Old","Desc");
            t.UpdateDetails("New","NewDesc");
            t.TestName.Should().Be("New");
            t.Description.Should().Be("NewDesc");
        }

        [Fact]
        public void AssignToLaboratory_Positive_SetsLaboratoryId()
        {
            var t = new LabTest("N","D");
            t.AssignToLaboratory(5);
            t.LaboratoryId.Should().Be(5);
        }

        [Fact]
        public void AssignToLaboratory_NonPositive_Throws()
        {
            var t = new LabTest("N","D");
            Action act = () => t.AssignToLaboratory(0);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void RemoveFromLaboratory_SetsNull()
        {
            var t = new LabTest("N","D");
            t.AssignToLaboratory(2);
            t.RemoveFromLaboratory();
            t.LaboratoryId.Should().BeNull();
        }
    }
}
