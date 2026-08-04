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
        private readonly INotificationService _notificationService;

        public RequestLabsService(IUnitOfWork uow, IMapper mapper, INotificationService notificationService)
        {
            _uow = uow;
            _mapper = mapper;
            _notificationService = notificationService;
        }
        // Backwards-compatible constructor used by tests and callers that don't provide notifications
        public RequestLabsService(IUnitOfWork uow, IMapper mapper)
            : this(uow, mapper, null)
        {
        }

        public async Task<RequestLabsReadDto> CreateAsync(RequestLabsCreateDto dto)
        {
            var session = await _uow.Sessions.GetByIdAsync(dto.SessionId)
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
            // Notify lab technicians belonging to the laboratories of requested tests
            var labIds = entity.LabTests
                .Select(lt => lt.LaboratoryId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct();

            var testNames = entity.LabTests.Select(lt => lt.TestName).ToList();

            foreach (var labId in labIds)
            {
                // fetch technicians for the laboratory (fetch up to 20 for simplicity)
                var techsPage = await _uow.LabTechnicians.GetByLaboratoryIdAsync(labId, new Domain.Models.PaginationParams(1, 20));
                foreach (var tech in techsPage.Items)
                {
                    try
                    {
                        if (!tech.ReceiveNotifications)
                            continue;

                        var user = await _uow.UserGeneric.GetByUserIdAsync(tech.Id.ToString());
                        if (user == null || !user.ReceiveNotifications)
                            continue;

                        var message = $"New lab request #{entity.Id} (session #{entity.SessionId})\nTests: {string.Join(", ", testNames)}\nRequestedAt: {entity.RequestedAt:u}";
                        if (_notificationService != null)
                            await _notificationService.SendToUserAsync(user.Id, message);
                    }
                    catch
                    {
                        // swallow individual notification failures to avoid affecting main flow
                    }
                }
            }
            return _mapper.Map<RequestLabsReadDto>(entity);
        }

        public async Task<RequestLabsReadDto> UpdateStatusAsync(int id, RequestLabsUpdateStatusDto dto)
        {
            var entity = await _uow.RequestLabs.GetByIdAsync(id)
                ?? throw new NotFoundException("RequestLabs", id);

            entity.UpdateStatus(dto.Status);
            await _uow.RequestLabs.UpdateAsync(entity);
            return _mapper.Map<RequestLabsReadDto>(entity);
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

        public async Task<PaginatedResult<RequestLabsReadDto>> QueryAsync(PaginationParams pagination, string? search = null, Domain.Enums.LabRequestStatus? status = null, Domain.Enums.LabRequestPriority? priority = null, int? labTestId = null, int? doctorId = null)
        {
            var page = await _uow.RequestLabs.QueryPaginatedAsync(pagination, search, status, priority, labTestId, doctorId);
            return PaginatedResult<RequestLabsReadDto>.Create(
                _mapper.Map<IEnumerable<RequestLabsReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task<RequestLabsStatisticsDto> GetStatisticsAsync()
        {
            var stats = await _uow.RequestLabs.GetStatisticsAsync();
            return new RequestLabsStatisticsDto
            {
                TotalRequests = stats.TotalRequests,
                PendingRequests = stats.PendingRequests,
                CompletedToday = stats.CompletedToday
            };
        }
    }
}
