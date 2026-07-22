using System;
using System.Threading.Tasks;
using Application.Services;
using Application.DTOs.Doctor;
using AutoMapper;
using Domain.IRepository;
using FluentAssertions;
using Moq;
using Xunit;
using Application.Common;

namespace Testing.Application
{
    public class DoctorServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<IDoctorRepo> _doctorRepo = new();
        private readonly Mock<Domain.IRepository.IPersonGenericRepo> _personRepo = new();
        private readonly Mock<IMapper> _mapper = new();

        public DoctorServiceTests()
        {
            _uow.Setup(u => u.Doctors).Returns(_doctorRepo.Object);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ThrowsNotFoundException()
        {
            _doctorRepo.Setup(d => d.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Domain.Entities.Doctor?)null);
            var svc = new DoctorService(_uow.Object, _personRepo.Object, _mapper.Object);
            Func<Task> act = async () => await svc.GetByIdAsync(1);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetByIdAsync_Found_ReturnsDto()
        {
            var doc = new Domain.Entities.Doctor("John Doe","S","C", Domain.Enums.Gender.Male, 1);
            _doctorRepo.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(doc);
            var dto = new DoctorReadDto { Id = 1, Name = doc.Name };
            _mapper.Setup(m => m.Map<DoctorReadDto>(doc)).Returns(dto);
            var svc = new DoctorService(_uow.Object, _personRepo.Object, _mapper.Object);
            var result = await svc.GetByIdAsync(1);
            result.Should().BeEquivalentTo(dto);
        }
    }
}
