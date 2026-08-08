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
        private readonly IEmailSender _emailSender;

        public RequestLabsService(
            IUnitOfWork uow,
            IMapper mapper,
            INotificationService notificationService,
            IEmailSender emailSender)
        {
            _uow = uow;
            _mapper = mapper;
            _notificationService = notificationService;
            _emailSender = emailSender;
        }

        // Backwards-compatible constructor used by tests
        public RequestLabsService(
            IUnitOfWork uow,
            IMapper mapper)
            : this(uow, mapper, null, null)
        {
        }

        public async Task<RequestLabsReadDto> CreateAsync(
            RequestLabsCreateDto dto)
        {
            var session =
                await _uow.Sessions.GetByIdAsync(dto.SessionId)
                ?? throw new NotFoundException(
                    "Session",
                    dto.SessionId);

            if (dto.LabTestIds is null ||
                dto.LabTestIds.Count == 0)
            {
                throw new ArgumentException(
                    "At least one lab test must be requested.");
            }

            var entity = new Domain.Entities.RequestLabs(
                dto.SessionId,
                dto.RequestedAt,
                dto.Priority);

            foreach (var labTestId in dto.LabTestIds.Distinct())
            {
                var labTest =
                    await _uow.LabTests.GetByIdAsync(labTestId)
                    ?? throw new NotFoundException(
                        "LabTest",
                        labTestId);

                entity.RequestLabTests.Add(
                    new Domain.Entities.RequestLabTest
                    {
                        LabTest = labTest,
                        LabTestId = labTest.Id,
                        CreatedAt = System.DateTime.UtcNow
                    });
            }

            await _uow.RequestLabs.AddAsync(entity);

            // Get laboratories related to requested tests
            var labIds = entity.RequestLabTests
                .Select(rlt => rlt.LabTest.LaboratoryId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct();

            var testNames = entity.RequestLabTests
                .Select(rlt => rlt.LabTest.TestName)
                .ToList();

            foreach (var labId in labIds)
            {
                var techsPage =
                    await _uow.LabTechnicians
                        .GetByLaboratoryIdAsync(
                            labId,
                            new Domain.Models.PaginationParams(
                                1,
                                20));

                foreach (var tech in techsPage.Items)
                {
                    try
                    {
                        if (!tech.ReceiveNotifications)
                            continue;

                        var user =
                            await _uow.UserGeneric
                                .GetByUserIdAsync(
                                    tech.Id.ToString());

                        if (user == null ||
                            !user.ReceiveNotifications)
                        {
                            continue;
                        }

                        // Notification message
                        var message =
                            $"New lab request #{entity.Id} " +
                            $"(session #{entity.SessionId})\n" +
                            $"Tests: {string.Join(", ", testNames)}\n" +
                            $"RequestedAt: {entity.RequestedAt:u}";

                        // ==========================================
                        // Send In-App Notification
                        // ==========================================
                        if (_notificationService != null)
                        {
                            await _notificationService
                                .SendToUserAsync(
                                    user.Id,
                                    message);
                        }

                        // ==========================================
                        // Send Email
                        // ==========================================
                        if (!string.IsNullOrWhiteSpace(tech.Email))
                        {
                            var emailMessage =
                                new Application.DTOs.Email.Message(
                                    new List<string>
                                    {
                                        tech.Email
                                    },
                                    $"New Lab Request #{entity.Id}",
                                    $@"
                                        <h2>New Laboratory Request</h2>

                                        <p>
                                            You have received a new
                                            laboratory request.
                                        </p>

                                        <p>
                                            <strong>Request ID:</strong>
                                            #{entity.Id}
                                        </p>

                                        <p>
                                            <strong>Session ID:</strong>
                                            #{entity.SessionId}
                                        </p>

                                        <p>
                                            <strong>Tests:</strong>
                                            {string.Join(", ", testNames)}
                                        </p>

                                        <p>
                                            <strong>Requested At:</strong>
                                            {entity.RequestedAt:u}
                                        </p>

                                        <p>
                                            Please log in to the system
                                            to view the request.
                                        </p>
                                    ");

                            await _emailSender
                                .SendEmailAsync(emailMessage);
                        }
                    }
                    catch
                    {
                        // Ignore notification/email failures
                    }
                }
            }

            // Reload entity with all navigation properties
            var createdRequest =
                await _uow.RequestLabs
                    .GetWithLabTestsAsync(entity.Id)
                ?? throw new NotFoundException(
                    "RequestLabs",
                    entity.Id);

            return _mapper.Map<RequestLabsReadDto>(
                createdRequest);
        }

        public async Task<RequestLabsReadDto> UpdateStatusAsync(
            int id,
            RequestLabsUpdateStatusDto dto)
        {
            var entity =
                await _uow.RequestLabs.GetByIdAsync(id)
                ?? throw new NotFoundException(
                    "RequestLabs",
                    id);

            entity.UpdateStatus(dto.Status);

            await _uow.RequestLabs.UpdateAsync(entity);

            var updatedRequest =
                await _uow.RequestLabs
                    .GetWithLabTestsAsync(id)
                ?? throw new NotFoundException(
                    "RequestLabs",
                    id);

            return _mapper.Map<RequestLabsReadDto>(
                updatedRequest);
        }

        public async Task<RequestLabsReadDto> GetByIdAsync(
            int id)
        {
            var entity =
                await _uow.RequestLabs
                    .GetWithLabTestsAsync(id)
                ?? throw new NotFoundException(
                    "RequestLabs",
                    id);

            // If all RequestLabTests are completed,
            // mark the parent RequestLabs as Completed.
            var rlt = entity.RequestLabTests;

            if (rlt != null &&
                rlt.Any() &&
                rlt.All(x =>
                    x.Status ==
                    Domain.Enums.RequestLabTestStatus.Completed))
            {
                if (entity.Status !=
                    Domain.Enums.LabRequestStatus.Completed)
                {
                    entity.UpdateStatus(
                        Domain.Enums.LabRequestStatus.Completed);

                    await _uow.RequestLabs
                        .UpdateAsync(entity);
                }
            }

            return _mapper.Map<RequestLabsReadDto>(
                entity);
        }

        public async Task<PaginatedResult<RequestLabsReadDto>>
            GetBySessionAsync(
                int sessionId,
                PaginationParams pagination)
        {
            var page =
                await _uow.RequestLabs
                    .GetBySessionPaginatedAsync(
                        sessionId,
                        pagination);

            return PaginatedResult<RequestLabsReadDto>.Create(
                _mapper.Map<IEnumerable<RequestLabsReadDto>>(
                    page.Items),
                page.TotalCount,
                pagination);
        }

        public async Task<PaginatedResult<RequestLabsReadDto>>
            QueryAsync(
                PaginationParams pagination,
                string? search = null,
                Domain.Enums.LabRequestStatus? status = null,
                Domain.Enums.LabRequestPriority? priority = null,
                int? labTestId = null,
                int? doctorId = null)
        {
            var page =
                await _uow.RequestLabs
                    .QueryPaginatedAsync(
                        pagination,
                        search,
                        status,
                        priority,
                        labTestId,
                        doctorId);

            return PaginatedResult<RequestLabsReadDto>.Create(
                _mapper.Map<IEnumerable<RequestLabsReadDto>>(
                    page.Items),
                page.TotalCount,
                pagination);
        }

        public async Task<RequestLabsStatisticsDto>
            GetStatisticsAsync()
        {
            var stats =
                await _uow.RequestLabs
                    .GetStatisticsAsync();

            return new RequestLabsStatisticsDto
            {
                TotalRequests = stats.TotalRequests,
                PendingRequests = stats.PendingRequests,
                CompletedToday = stats.CompletedToday
            };
        }

        public async Task<PaginatedResult<RequestLabsReadDto>>
            GetPendingRequestsAsync(
                PaginationParams pagination)
        {
            var page =
                await _uow.RequestLabs
                    .GetByStatusPaginatedAsync(
                        Domain.Enums.LabRequestStatus.Pending,
                        pagination);

            return PaginatedResult<RequestLabsReadDto>.Create(
                _mapper.Map<IEnumerable<RequestLabsReadDto>>(
                    page.Items),
                page.TotalCount,
                pagination);
        }

        public async Task<PaginatedResult<RequestLabsReadDto>>
            GetByStatusAsync(
                Domain.Enums.LabRequestStatus status,
                PaginationParams pagination)
        {
            var page =
                await _uow.RequestLabs
                    .GetByStatusPaginatedAsync(
                        status,
                        pagination);

            return PaginatedResult<RequestLabsReadDto>.Create(
                _mapper.Map<IEnumerable<RequestLabsReadDto>>(
                    page.Items),
                page.TotalCount,
                pagination);
        }

        public async Task<RequestLabsReadDto>
            CheckIf_RequestLabTestsCompleted(
                int requestLabId)
        {
            var requestLab =
                await _uow.RequestLabs
                    .GetWithLabTestsAsync(requestLabId)
                ?? throw new NotFoundException(
                    "RequestLabs",
                    requestLabId);

            foreach (var requestLabTest
                in requestLab.RequestLabTests)
            {
                if (requestLabTest.Status !=
                    Domain.Enums.RequestLabTestStatus.Completed)
                {
                    var reqLab =
                        _mapper.Map<RequestLabsReadDto>(
                            requestLab);

                    return reqLab;
                }
            }

            var entity =
                await _uow.RequestLabs
                    .GetByIdAsync(requestLabId)
                ?? throw new NotFoundException(
                    "RequestLabs",
                    requestLabId);

            entity.UpdateStatus(
                Domain.Enums.LabRequestStatus.Completed);

            await _uow.RequestLabs
                .UpdateAsync(entity);

            var updatedRequest =
                await _uow.RequestLabs
                    .GetWithLabTestsAsync(requestLabId)
                ?? throw new NotFoundException(
                    "RequestLabs",
                    requestLabId);

            return _mapper.Map<RequestLabsReadDto>(
                updatedRequest);
        }
    }
}