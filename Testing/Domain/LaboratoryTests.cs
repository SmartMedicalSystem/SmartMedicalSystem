using System;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Testing.Domain
{
    public class LaboratoryTests
    {
        [Fact]
        public void Constructor_InvalidPhone_Throws()
        {
            Action act = () => new Laboratory("Lab","Loc","123", LabStatus.Active);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateDetails_ChangeStatusAndFields()
        {
            var lab = new Laboratory("L","Loc","01012345678", LabStatus.Active);
            lab.UpdateDetails("New","NewLoc","01112345678", LabStatus.Inactive, 5, 2, "C","S");
            lab.Name.Should().Be("New");
            lab.Phone.Should().Be("01112345678");
            lab.Status.Should().Be(LabStatus.Inactive);
            lab.HeadTechnicianId.Should().Be(5);
            lab.DepartmentId.Should().Be(2);
            lab.Code.Should().Be("C");
            lab.Specialty.Should().Be("S");
        }

        [Fact]
        public void AssignHeadTechnician_NonPositive_Throws()
        {
            var lab = new Laboratory("L","Loc","01012345678", LabStatus.Active);
            Action act = () => lab.AssignHeadTechnician(0);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AddRemoveTechnician_WorksAndValidates()
        {
            var lab = new Laboratory("L","Loc","01012345678", LabStatus.Active);
            var tech = new LabTechnician("Name","01012345678");
            lab.AddTechnician(tech);
            lab.LabTechnicians.Should().Contain(tech);
            lab.RemoveTechnician(tech);
            lab.LabTechnicians.Should().NotContain(tech);
        }

        [Fact]
        public void AddTechnician_Null_Throws()
        {
            var lab = new Laboratory("L","Loc","01012345678", LabStatus.Active);
            Action act = () => lab.AddTechnician(null!);
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
