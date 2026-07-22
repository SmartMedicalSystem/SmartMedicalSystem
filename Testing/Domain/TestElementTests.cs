using System;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Testing.Domain
{
    public class TestElementTests
    {
        [Fact]
        public void Constructor_Valid_SetsProperties()
        {
            var e = new TestElement("Hemoglobin", "g/dL", 10f, 18f);
            e.ElementName.Should().Be("Hemoglobin");
            e.Unit.Should().Be("g/dL");
            e.NormalMin.Should().Be(10f);
            e.NormalMax.Should().Be(18f);
        }

        [Fact]
        public void Constructor_InvalidRange_Throws()
        {
            Action act = () => new TestElement("X","u", 5f, 5f);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateRange_Invalid_Throws()
        {
            var e = new TestElement("T","u",1f,2f);
            Action act = () => e.UpdateRange(5f, 4f);
            act.Should().Throw<ArgumentException>();
        }
    }
}
