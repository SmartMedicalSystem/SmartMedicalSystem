using Application.Common;
using Application.DTOs.AI;
using Application.DTOs.PatientResult;
using Application.Services.Abstraction;
using Application.Services.Abstraction.AI;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PatientResultService : IPatientResultService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPatientResultAIService? _patientResultAIService;
        private readonly INotificationService? _notificationService;

        public PatientResultService(IUnitOfWork uow, IMapper mapper, IPatientResultAIService? patientResultAIService = null, INotificationService? notificationService = null)
        {
            _uow = uow;
            _mapper = mapper;
            _patientResultAIService = patientResultAIService;
            _notificationService = notificationService;
        }

        public Task<PatientResultAIAnalysisDto> GenerateAIAnalysisAsync(int id, CancellationToken cancellationToken = default)
        {
            if (_patientResultAIService == null)
                throw new InvalidOperationException("AI service not configured. Provide IPatientResultAIService to use GenerateAIAnalysisAsync.");
            return _patientResultAIService.GenerateAnalysisAsync(id, cancellationToken);
        }

        public async Task<PatientResultReadDto> CreateAsync(PatientResultCreateDto dto)
        {
            _ = await _uow.Patients.GetByIdAsync(dto.PatientId) ?? throw new NotFoundException("Patient", dto.PatientId);
            var session = await _uow.Sessions.GetByIdAsync(dto.SessionId) ?? throw new NotFoundException("Session", dto.SessionId);
            _ = await _uow.LabTests.GetByIdAsync(dto.LabTestId) ?? throw new NotFoundException("LabTest", dto.LabTestId);

            var entity = new Domain.Entities.PatientResult(
                dto.PatientId, dto.SessionId, dto.LabTestId, dto.Summary, dto.AIClassifiedReport, dto.AISuggestion);

            await _uow.PatientResults.AddAsync(entity);

            // Notify doctor that results are ready
            try
            {
                if (_notificationService != null && session.Doctor != null && session.Doctor.ReceiveNotifications)
                {
                    var user = await _uow.UserGeneric.GetByUserIdAsync(session.Doctor.Id.ToString());
                    if (user != null && user.ReceiveNotifications)
                    {
                        var message = $"Results ready for session #{session.Id}. LabTestId: {dto.LabTestId}. ResultId: {entity.Id}";
                        await _notificationService.SendToUserAsync(user.Id, message);
                    }
                }
            }
            catch
            {
                // ignore notification failures
            }
            return _mapper.Map<PatientResultReadDto>(entity);
        }

        public async Task<PatientResultReadDto> UpdateAsync(int id, PatientResultUpdateDto dto)
        {
            var entity = await _uow.PatientResults.GetByIdAsync(id)
                ?? throw new NotFoundException("PatientResult", id);

            entity.UpdateAIOutput(dto.AIClassifiedReport, dto.AISuggestion, dto.Summary);
            await _uow.PatientResults.UpdateAsync(entity);
            return _mapper.Map<PatientResultReadDto>(entity);
        }

        public async Task<PatientResultReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.PatientResults.GetWithResultElementsAsync(id)
                ?? throw new NotFoundException("PatientResult", id);
            return _mapper.Map<PatientResultReadDto>(entity);
        }

        public async Task<PaginatedResult<PatientResultReadDto>> GetByPatientAsync(int patientId, PaginationParams pagination)
        {
            var page = await _uow.PatientResults.GetByPatientPaginatedAsync(patientId, pagination);
            return PaginatedResult<PatientResultReadDto>.Create(
                _mapper.Map<IEnumerable<PatientResultReadDto>>(page.Items),
                page.TotalCount, pagination);
        }
    }
}
