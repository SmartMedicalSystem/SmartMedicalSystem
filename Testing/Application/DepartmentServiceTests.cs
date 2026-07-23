using System;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.Department;
using Application.Services;
using Domain.IRepository;
using FluentAssertions;
using Moq;
using Xunit;

namespace Testing.Application
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<IDepartmentRepo> _deptRepo = new();
        private readonly Mock<IDoctorRepo> _doctorRepo = new();

        public DepartmentServiceTests()
        {
            _uow.Setup(u => u.Departments)
                .Returns(_deptRepo.Object);

            _uow.Setup(u => u.Doctors)
                .Returns(_doctorRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_NameNotUnique_ThrowsArgumentException()
        {
            // Arrange
            _deptRepo
                .Setup(d => d.IsDepartmentNameUniqueAsync(
                    It.IsAny<string>(),
                    null))
                .ReturnsAsync(false);

            var svc = new DepartmentService(_uow.Object);

            // Act
            Func<Task> act = async () =>
                await svc.CreateAsync(
                    new DepartmentCreateDto
                    {
                        Name = "X"
                    });

            // Assert
            await act
                .Should()
                .ThrowAsync<ArgumentException>();
        }


        [Fact]
        public async Task CreateAsync_HeadDoctorNotFound_ThrowsNotFoundException()
        {
            // Arrange
            _deptRepo
                .Setup(d => d.IsDepartmentNameUniqueAsync(
                    It.IsAny<string>(),
                    null))
                .ReturnsAsync(true);

            _deptRepo
                .Setup(d => d.AddAsync(
                    It.IsAny<Domain.Entities.Department>()))
                .ReturnsAsync(
                    (Domain.Entities.Department e) => e);

            _uow
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            _doctorRepo
                .Setup(d => d.GetByIdAsync(
                    It.IsAny<int>()))
                .ReturnsAsync(
                    (Domain.Entities.Doctor?)null);

            var svc = new DepartmentService(_uow.Object);

            // Act
            var dto = new DepartmentCreateDto
            {
                Name = "D",
                HeadDoctorId = 5,
                HeadDoctor = "Dr"
            };

            Func<Task> act = async () =>
                await svc.CreateAsync(dto);

            // Assert
            await act
                .Should()
                .ThrowAsync<NotFoundException>();
        }


        [Fact]
        public async Task CreateAsync_Valid_CreatesAndReturnsDto()
        {
            // Arrange
            _deptRepo
                .Setup(d => d.IsDepartmentNameUniqueAsync(
                    It.IsAny<string>(),
                    null))
                .ReturnsAsync(true);

            _deptRepo
                .Setup(d => d.AddAsync(
                    It.IsAny<Domain.Entities.Department>()))
                .ReturnsAsync(
                    (Domain.Entities.Department e) => e);

            _uow
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            var svc = new DepartmentService(_uow.Object);

            // Act
            var result = await svc.CreateAsync(
                new DepartmentCreateDto
                {
                    Name = "D",
                    HeadDoctor = "Dr Head"
                });

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("D");

            _deptRepo.Verify(
                d => d.AddAsync(
                    It.IsAny<Domain.Entities.Department>()),
                Times.Once);

            _uow.Verify(
                u => u.SaveChangesAsync(),
                Times.Once);
        }
    }
}