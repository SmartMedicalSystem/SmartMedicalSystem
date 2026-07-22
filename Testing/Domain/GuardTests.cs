using System;
using Domain.Common;
using FluentAssertions;
using Xunit;

namespace Testing.Domain
{
    public class GuardTests
    {
        [Fact]
        public void NotNullOrWhiteSpace_NullOrWhitespace_Throws()
        {
            // Arrange
            // Act
            Action actNull = () => Guard.NotNullOrWhiteSpace(null, "param");
            Action actEmpty = () => Guard.NotNullOrWhiteSpace("   ", "param");

            // Assert
            actNull.Should().Throw<ArgumentException>().WithMessage("param is required.*");
            actEmpty.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void NotNullOrWhiteSpace_TooLong_Throws()
        {
            // Arrange
            var longValue = new string('a', 201);

            // Act
            Action act = () => Guard.NotNullOrWhiteSpace(longValue, "param", 200);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("param cannot exceed 200 characters.*");
        }

        [Fact]
        public void NotNullOrWhiteSpace_Trimmed_ReturnsTrimmed()
        {
            // Arrange
            var input = "  hello  ";

            // Act
            var result = Guard.NotNullOrWhiteSpace(input, "p");

            // Assert
            result.Should().Be("hello");
        }

        [Fact]
        public void Positive_NonPositive_Throws()
        {
            Action act0 = () => Guard.Positive(0, "n");
            Action actNeg = () => Guard.Positive(-5, "n");

            act0.Should().Throw<ArgumentException>();
            actNeg.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Positive_Positive_ReturnsValue()
        {
            var v = Guard.Positive(5, "n");
            v.Should().Be(5);
        }

        [Fact]
        public void PositiveOrZero_Negative_Throws()
        {
            Action act = () => Guard.PositiveOrZero(-0.1f, "f");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void NotDefault_DefaultDate_Throws()
        {
            Action act = () => Guard.NotDefault(default, "d");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void NotInFuture_FutureDate_Throws()
        {
            var future = DateTime.UtcNow.AddDays(1);
            Action act = () => Guard.NotInFuture(future, "d");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Range_MinGreaterOrEqualMax_Throws()
        {
            Action act = () => Guard.Range(5, 5, "min", "max");
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("01012345678")]
        [InlineData("01112345678")]
        [InlineData("01212345678")]
        [InlineData("01512345678")]
        public void ValidatePhone_ValidNumbers_ReturnsSame(string phone)
        {
            var result = Guard.ValidatePhone(phone);
            result.Should().Be(phone);
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("01912345678")]
        public void ValidatePhone_Invalid_Throws(string phone)
        {
            Action act = () => Guard.ValidatePhone(phone);
            act.Should().Throw<ArgumentException>();
        }
    }
}
