using System;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Testing.Domain
{
    public class LabTechProfileTests
    {
        [Fact]
        public void Constructor_Valid_SetsProperties()
        {
            var dob = DateTime.Today.AddYears(-25);
            var profile = new LabTechProfile(
                userId: 1,
                firstName: "F",
                lastName: "L",
                gender: Gender.Male,
                dateOfBirth: dob,
                nationality: "N",
                nationalId: "NID",
                employeeId: "E1",
                assignedLaboratory: "Lab",
                jobTitle: "Tech",
                yearsOfExperience: 3,
                joiningDate: DateTime.Today,
                email: "a@b.com",
                phoneNumber: "01012345678",
                address: "addr"
            );

            profile.UserId.Should().Be(1);
            profile.FirstName.Should().Be("F");
            profile.Email.Should().Be("a@b.com");
        }

        [Fact]
        public void UpdatePersonalInfo_Invalid_Throws()
        {
            var dob = DateTime.Today.AddYears(-25);
            var profile = new LabTechProfile(1, "F", "L", Gender.Male, dob, "N", "ID", "E1", "Lab", "J", 1, DateTime.Today, "a@b.com", "01012345678", "addr");
            Action act = () => profile.UpdatePersonalInfo("", "L", Gender.Male, dob, "N", "ID");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateProfilePicture_Null_SetsNull()
        {
            var dob = DateTime.Today.AddYears(-25);
            var profile = new LabTechProfile(1, "F", "L", Gender.Male, dob, "N", "ID", "E1", "Lab", "J", 1, DateTime.Today, "a@b.com", "01012345678", "addr");
            profile.UpdateProfilePicture(null);
            profile.ProfilePictureUrl.Should().BeNull();
            profile.UpdateProfilePicture("  url  ");
            profile.ProfilePictureUrl.Should().Be("url");
        }
    }
}
