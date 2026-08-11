using System;
using System.Threading.Tasks;
using Application.Services;
using Application.Common;
using AutoMapper;
using Domain.IRepository;
using FluentAssertions;
using Moq;
using Xunit;
using Application.DTOs.Patient;

namespace Testing.Application
{
    public class PatientServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<Domain.IRepository.IPersonGenericRepo> _personRepo = new();
        private readonly Mock<IPatientRepo> _patientRepo = new();
        private readonly Mock<IMapper> _mapper = new();

        public PatientServiceTests()
        {
            _uow.Setup(u => u.Patients).Returns(_patientRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_Valid_CallsAddPersonAndReturnsDto()
        {
            var dto = new PatientCreateDto { FirstName = "A", LastName = "B", DateOfBirth = DateTime.Today.AddYears(-20), NationalId = "N1" };
            _personRepo.Setup(p => p.AddPerson(It.IsAny<string>(), It.IsAny<Domain.Entities.Patient>())).Returns(Task.CompletedTask);
            var readDto = new PatientReadDto { Id = 1, FirstName = "A" };
            _mapper.Setup(m => m.Map<PatientReadDto>(It.IsAny<Domain.Entities.Patient>())).Returns(readDto);
            var svc = new PatientService(_uow.Object, _personRepo.Object, _mapper.Object);

            var result = await svc.CreateAsync(dto);

            result.Should().BeEquivalentTo(readDto);
            _personRepo.Verify(p => p.AddPerson(dto.NationalId, It.IsAny<Domain.Entities.Patient>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ThrowsNotFoundException()
        {
            _patientRepo.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Domain.Entities.Patient?)null);
            var svc = new PatientService(_uow.Object, _personRepo.Object, _mapper.Object);
            Func<Task> act = async () => await svc.GetByIdAsync(1);
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
