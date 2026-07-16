using Application.Common;
using Application.DTOs.RequestLabs;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RequestLabsService : IRequestLabsService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public RequestLabsService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<RequestLabsReadDto> CreateAsync(RequestLabsCreateDto dto)
        {
            _ = await _uow.Sessions.GetByIdAsync(dto.SessionId)
                ?? throw new NotFoundException("Session", dto.SessionId);

            if (dto.LabTestIds is null || dto.LabTestIds.Count == 0)
                throw new System.ArgumentException("At least one lab test must be requested.");

            var entity = new Domain.Entities.RequestLabs(dto.SessionId, dto.RequestedAt, dto.Priority);

            foreach (var labTestId in dto.LabTestIds.Distinct())
            {
                var labTest = await _uow.LabTests.GetByIdAsync(labTestId)
                    ?? throw new NotFoundException("LabTest", labTestId);
                entity.LabTests.Add(labTest);
            }

            await _uow.RequestLabs.AddAsync(entity);

            // Re-fetch with Patient/Doctor/Department included so the mapper has everything it needs.
            var created = await _uow.RequestLabs.GetWithLabTestsAsync(entity.Id)
                ?? throw new NotFoundException("RequestLabs", entity.Id);

            return _mapper.Map<RequestLabsReadDto>(created);
        }

        public async Task<RequestLabsReadDto> UpdateStatusAsync(int id, RequestLabsUpdateStatusDto dto)
        {
            var entity = await _uow.RequestLabs.GetByIdAsync(id)
                ?? throw new NotFoundException("RequestLabs", id);

            entity.UpdateStatus(dto.Status);
            await _uow.RequestLabs.UpdateAsync(entity);

            var updated = await _uow.RequestLabs.GetWithLabTestsAsync(id)
                ?? throw new NotFoundException("RequestLabs", id);

            return _mapper.Map<RequestLabsReadDto>(updated);
        }

        public async Task<RequestLabsReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.RequestLabs.GetWithLabTestsAsync(id)
                ?? throw new NotFoundException("RequestLabs", id);
            return _mapper.Map<RequestLabsReadDto>(entity);
        }

        public async Task<PaginatedResult<RequestLabsReadDto>> GetBySessionAsync(int sessionId, PaginationParams pagination)
        {
            var page = await _uow.RequestLabs.GetBySessionPaginatedAsync(sessionId, pagination);
            return PaginatedResult<RequestLabsReadDto>.Create(
                _mapper.Map<IEnumerable<RequestLabsReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task<PaginatedResult<RequestLabsReadDto>> GetFilteredAsync(RequestLabsFilterDto filter, PaginationParams pagination)
        {
            var filterParams = new RequestLabsFilterParams
            {
                Status = filter.Status,
                Priority = filter.Priority,
                LabTestId = filter.LabTestId,
                DoctorId = filter.DoctorId,
                SearchTerm = filter.SearchTerm
            };

            var page = await _uow.RequestLabs.GetFilteredAsync(filterParams, pagination);

            return PaginatedResult<RequestLabsReadDto>.Create(
                _mapper.Map<IEnumerable<RequestLabsReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task<RequestLabsStatsDto> GetStatsAsync()
        {
            var stats = await _uow.RequestLabs.GetStatsAsync();

            return new RequestLabsStatsDto
            {
                TotalRequests = stats.TotalRequests,
                PendingCount = stats.PendingCount,
                CompletedTodayCount = stats.CompletedTodayCount
            };
        }
    }
}