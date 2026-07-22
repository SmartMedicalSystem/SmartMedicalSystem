using System;
using System.Threading.Tasks;
using Application.Services;
using Application.Common;
using Application.DTOs.PatientResult;
using AutoMapper;
using Domain.IRepository;
using FluentAssertions;
using Moq;
using Xunit;
using Application.DTOs.PatientResult;

namespace Testing.Application
{
    public class PatientResultServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<IPatientRepo> _patientRepo = new();
        private readonly Mock<ISessionRepo> _sessionRepo = new();
        private readonly Mock<ILabTestRepo> _labTestRepo = new();
        private readonly Mock<IPatientResultRepo> _patientResultRepo = new();
        private readonly Mock<IMapper> _mapper = new();

        public PatientResultServiceTests()
        {
            _uow.Setup(u => u.Patients).Returns(_patientRepo.Object);
            _uow.Setup(u => u.Sessions).Returns(_sessionRepo.Object);
            _uow.Setup(u => u.LabTests).Returns(_labTestRepo.Object);
            _uow.Setup(u => u.PatientResults).Returns(_patientResultRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_PatientMissing_ThrowsNotFound()
        {
            _patientRepo.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Domain.Entities.Patient?)null);
            var svc = new PatientResultService(_uow.Object, _mapper.Object);
            Func<Task> act = async () => await svc.CreateAsync(new PatientResultCreateDto { PatientId = 1, SessionId = 1, LabTestId = 1 });
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_Valid_AddsAndReturnsDto()
        {
            _patientRepo.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Patient("F","L",DateTime.Today.AddYears(-20)));
            _sessionRepo.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Session(1,1,1,DateTime.UtcNow));
            _labTestRepo.Setup(l => l.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.LabTest("T","D"));
            _patientResultRepo.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.PatientResult>())).ReturnsAsync((Domain.Entities.PatientResult e) => e ?? new Domain.Entities.PatientResult(1,1,1,null,null,null));
            _mapper.Setup(m => m.Map<PatientResultReadDto>(It.IsAny<Domain.Entities.PatientResult>())).Returns(new PatientResultReadDto { Id = 1 });
            var svc = new PatientResultService(_uow.Object, _mapper.Object);
            var dto = new PatientResultCreateDto { PatientId = 1, SessionId = 1, LabTestId = 1, Summary = "s" };
            var result = await svc.CreateAsync(dto);
            result.Should().NotBeNull();
            _patientResultRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.PatientResult>()), Times.Once);
        }
    }
}
