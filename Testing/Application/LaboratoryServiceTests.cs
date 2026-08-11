using System;
using System.Threading.Tasks;
using Application.Services;
using AutoMapper;
using Domain.IRepository;
using FluentAssertions;
using Moq;
using Xunit;
using Application.DTOs.Laboratory;

namespace Testing.Application
{
    public class LaboratoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<ILaboratoryRepo> _labRepo = new();
        private readonly Mock<IMapper> _mapper = new();

        public LaboratoryServiceTests()
        {
            _uow.Setup(u => u.Laboratories).Returns(_labRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_NameNotUnique_ThrowsArgumentException()
        {
            _labRepo.Setup(l => l.IsLaboratoryNameUniqueAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
            var svc = new LaboratoryService(_uow.Object, _mapper.Object);
            Func<Task> act = async () => await svc.CreateAsync(new LaboratoryCreateDto { Name = "L", Location = "Loc", Phone = "01012345678" });
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateAsync_Valid_ReturnsDto()
        {
            _labRepo.Setup(l => l.IsLaboratoryNameUniqueAsync(It.IsAny<string>(), null)).ReturnsAsync(true);
            _labRepo.Setup(l => l.AddAsync(It.IsAny<Domain.Entities.Laboratory>())).ReturnsAsync((Domain.Entities.Laboratory e) => e ?? new Domain.Entities.Laboratory("L","Loc","01012345678"));
            _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            var dto = new LaboratoryReadDto { Id = 1, Name = "L" };
            _mapper.Setup(m => m.Map<LaboratoryReadDto>(It.IsAny<Domain.Entities.Laboratory>())).Returns(dto);
            var svc = new LaboratoryService(_uow.Object, _mapper.Object);
            var result = await svc.CreateAsync(new LaboratoryCreateDto { Name = "L", Location = "Loc", Phone = "01012345678" });
            result.Should().BeEquivalentTo(dto);
            _labRepo.Verify(l => l.AddAsync(It.IsAny<Domain.Entities.Laboratory>()), Times.Once);
        }
    }
}
