using System;
using Domain.Entities.Person;
using FluentAssertions;
using Xunit;

namespace Testing.Domain
{
    public class BasePersonTests
    {
        [Fact]
        public void Constructor_Valid_SetsProperties()
        {
            // Arrange
            var dob = DateTime.Today.AddYears(-30);

            // Act
            var person = new TestPerson("John", "Doe", dob);

            // Assert
            person.FirstName.Should().Be("John");
            person.LastName.Should().Be("Doe");
            person.DateOfBirth.Should().Be(dob);
            person.Age.Should().Be(30);
        }

        [Fact]
        public void Constructor_FutureDob_Throws()
        {
            var future = DateTime.Today.AddDays(1);
            Action act = () => new TestPerson("A","B", future);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Name_SetSingleWord_SetsFirstNameOnly()
        {
            var p = new TestPerson("First","Last", DateTime.Today.AddYears(-20));
            p.Name = "Cher";
            p.FirstName.Should().Be("Cher");
            p.LastName.Should().BeEmpty();
        }

        [Fact]
        public void Name_SetEmpty_Throws()
        {
            var p = new TestPerson("F","L", DateTime.Today.AddYears(-20));
            Action act = () => p.Name = "   ";
            act.Should().Throw<ArgumentException>();
        }

        // Helper concrete class for testing since BasePerson is abstract
        private class TestPerson : BasePerson
        {
            public TestPerson(string firstName, string lastName, DateTime dob)
                : base(firstName, lastName, dob) { }
        }
    }
}
