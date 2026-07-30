using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Services;
using Application.Common;
using Application.DTOs.RequestLabs;
using AutoMapper;
using Domain.IRepository;
using FluentAssertions;
using Moq;
using Xunit;
using Application.DTOs.RequestLabs;

namespace Testing.Application
{
    public class RequestLabsServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<ISessionRepo> _sessionRepo = new();
        private readonly Mock<IRequestLabsRepo> _requestRepo = new();
        private readonly Mock<ILabTestRepo> _labTestRepo = new();
        private readonly Mock<IMapper> _mapper = new();

        public RequestLabsServiceTests()
        {
            _uow.Setup(u => u.Sessions).Returns(_sessionRepo.Object);
            _uow.Setup(u => u.RequestLabs).Returns(_requestRepo.Object);
            _uow.Setup(u => u.LabTests).Returns(_labTestRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_NoSession_ThrowsNotFound()
        {
            _sessionRepo.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Domain.Entities.Session?)null);
            var svc = new RequestLabsService(_uow.Object, _mapper.Object);
            Func<Task> act = async () => await svc.CreateAsync(new RequestLabsCreateDto { SessionId = 1, RequestedAt = DateTime.UtcNow, LabTestIds = new List<int> { 1 } });
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_NoLabTests_ThrowsArgumentException()
        {
            _sessionRepo.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Session(1,1,1,DateTime.UtcNow)); // test session setup
            var svc = new RequestLabsService(_uow.Object, _mapper.Object);
            Func<Task> act = async () => await svc.CreateAsync(new RequestLabsCreateDto { SessionId = 1, RequestedAt = DateTime.UtcNow, LabTestIds = new List<int>() });
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateAsync_Valid_AddsAndReturnsDto()
        {
            _sessionRepo.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Session(1,1,1,DateTime.UtcNow));
            _labTestRepo.Setup(l => l.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.LabTest("T","D"));
            _requestRepo.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.RequestLabs>())).ReturnsAsync((Domain.Entities.RequestLabs e) => e ?? new Domain.Entities.RequestLabs(1, DateTime.UtcNow));
            _mapper.Setup(m => m.Map<RequestLabsReadDto>(It.IsAny<Domain.Entities.RequestLabs>())).Returns(new RequestLabsReadDto { Id = 1 }); // refreshed
            var svc = new RequestLabsService(_uow.Object, _mapper.Object);
            var dto = new RequestLabsCreateDto { SessionId = 1, RequestedAt = DateTime.UtcNow, LabTestIds = new List<int> { 1, 1 } };
            var result = await svc.CreateAsync(dto);
            result.Should().NotBeNull();
            _requestRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.RequestLabs>()), Times.Once);
        }
    }
}
