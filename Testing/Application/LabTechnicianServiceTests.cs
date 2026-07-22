using System;
using System.Threading.Tasks;
using Application.Services;
using AutoMapper;
using Domain.IRepository;
using FluentAssertions;
using Moq;
using Xunit;
using Application.DTOs.LabTechnician;
using Application.DTOs.Auth;
using Application.Services.Abstraction.Auth;

namespace Testing.Application
{
    public class LabTechnicianServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<Domain.IRepository.IPersonGenericRepo> _personRepo = new();
        private readonly Mock<ILabTechnicianRepo> _labTechRepo = new();
        private readonly Mock<IFileStorageService> _fileStorage = new();
        private readonly Mock<IAuthService> _authService = new();
        private readonly Mock<IMapper> _mapper = new();

        public LabTechnicianServiceTests()
        {
            _uow.Setup(u => u.LabTechnicians).Returns(_labTechRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_NationalIdExists_Throws()
        {
            _labTechRepo.Setup(r => r.GetByNationalIdAsync(It.IsAny<string>())).ReturnsAsync(new Domain.Entities.LabTechnician());
            var svc = new LabTechnicianService(_uow.Object, _personRepo.Object, _mapper.Object, _fileStorage.Object, _authService.Object);
            Func<Task> act = async () => await svc.CreateAsync(new LabTechnicianCreateDto { NationalId = "N" });
            await act.Should().ThrowAsync<Exception>().WithMessage("National ID already exists.*");
        }

        [Fact]
        public async Task CreateAsync_Valid_CreatesAndCallsAuth()
        {
            _labTechRepo.Setup(r => r.GetByNationalIdAsync(It.IsAny<string>())).ReturnsAsync((Domain.Entities.LabTechnician?)null);
            _fileStorage.Setup(f => f.SaveImageAsync(It.IsAny<Microsoft.AspNetCore.Http.IFormFile?>())).ReturnsAsync("url");
            _uow.Setup(u => u.LabTechnicians.AddAsync(It.IsAny<Domain.Entities.LabTechnician>())).ReturnsAsync((Domain.Entities.LabTechnician e) => e ?? new Domain.Entities.LabTechnician());
            _authService.Setup(a => a.CreateUserAsync(It.IsAny<CreateUserRequestDto>())).ReturnsAsync(new AuthResponseDto { IsSuccess = true, AccessToken = "t" });
            var svc = new LabTechnicianService(_uow.Object, _personRepo.Object, _mapper.Object, _fileStorage.Object, _authService.Object);
            var formFile = new Mock<Microsoft.AspNetCore.Http.IFormFile>();
            var dto = new LabTechnicianCreateDto { NationalId = "N", FirstName = "F", LastName = "L", PhotoUrl = formFile.Object, Username = "u", Password = "pwd" };
            var result = await svc.CreateAsync(dto);
            _labTechRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.LabTechnician>()), Times.Once);
            _authService.Verify(a => a.CreateUserAsync(It.IsAny<CreateUserRequestDto>()), Times.Once);
        }
    }
}
