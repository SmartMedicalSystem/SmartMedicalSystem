using Domain.Models;
using FluentAssertions;
using Xunit;

namespace Testing.Domain
{
    public class PaginationParamsTests
    {
        [Fact]
        public void Defaults_PageNumber1_PageSize10()
        {
            var p = new PaginationParams();
            p.PageNumber.Should().Be(1);
            p.PageSize.Should().Be(10);
        }

        [Fact]
        public void PageNumber_SetZero_ResetsToOne()
        {
            var p = new PaginationParams();
            p.PageNumber = 0;
            p.PageNumber.Should().Be(1);
        }

        [Fact]
        public void PageSize_SetNegative_DefaultsTo10()
        {
            var p = new PaginationParams();
            p.PageSize = -5;
            p.PageSize.Should().Be(10);
        }

        [Fact]
        public void PageSize_ExceedsMax_Clamps()
        {
            var p = new PaginationParams();
            p.PageSize = PaginationParams.MaxPageSize + 100;
            p.PageSize.Should().Be(PaginationParams.MaxPageSize);
        }

        [Fact]
        public void CalculateSkip_CalculatesCorrectly()
        {
            var p = new PaginationParams(3, 20);
            p.CalculateSkip().Should().Be(40);
        }

        [Fact]
        public void IsValid_ReturnsExpected()
        {
            new PaginationParams(1, 10).IsValid().Should().BeTrue();

            // Constructor normalizes invalid inputs: pageNumber=0 -> becomes 1, pageSize=0 -> becomes 10
            var p1 = new PaginationParams(0, 10);
            p1.PageNumber.Should().Be(1);
            p1.IsValid().Should().BeTrue();

            var p2 = new PaginationParams(1, 0);
            p2.PageSize.Should().Be(10);
            p2.IsValid().Should().BeTrue();
        }
    }
}
