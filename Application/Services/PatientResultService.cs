using Application.Common;
using Application.DTOs.PatientResult;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Domain.Enums;

namespace Application.Services
{
    public class PatientResultService : IPatientResultService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PatientResultService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PatientResultReadDto> CreateAsync(PatientResultCreateDto dto)
        {
            _ = await _uow.Patients.GetByIdAsync(dto.PatientId) ?? throw new NotFoundException("Patient", dto.PatientId);
            _ = await _uow.Sessions.GetByIdAsync(dto.SessionId) ?? throw new NotFoundException("Session", dto.SessionId);
            _ = await _uow.LabTests.GetByIdAsync(dto.LabTestId) ?? throw new NotFoundException("LabTest", dto.LabTestId);

            // Validate provided result elements (technicians and test elements)
            foreach (var el in dto.ResultElements)
            {
                _ = await _uow.TestElements.GetByIdAsync(el.TestElementId) ?? throw new NotFoundException("TestElement", el.TestElementId);
                _ = await _uow.LabTechnicians.GetByIdAsync(el.TechId) ?? throw new NotFoundException("LabTechnician", el.TechId);
            }

            var entity = new Domain.Entities.PatientResult(
                dto.PatientId, dto.SessionId, dto.LabTestId, dto.Summary, dto.AIClassifiedReport, dto.AISuggestion, dto.Notes, dto.IsDraft);

            await _uow.PatientResults.AddAsync(entity);

            // Create result elements after PatientResult has an Id
            foreach (var el in dto.ResultElements)
            {
                var element = new Domain.Entities.PatientResultElement(entity.Id, el.TestElementId, el.Value, el.TechId);
                await _uow.PatientResultElements.AddAsync(element);
            }

            // If this is a final submission (not a draft) update related RequestLabs status to Completed
            if (!dto.IsDraft)
            {
                var requests = await _uow.RequestLabs.GetBySessionAsync(dto.SessionId);
                var matching = requests.FirstOrDefault(r => r.LabTests.Any(lt => lt.Id == dto.LabTestId));
                if (matching != null)
                {
                    matching.UpdateStatus(LabRequestStatus.Completed);
                    await _uow.RequestLabs.UpdateAsync(matching);
                }
            }

            return _mapper.Map<PatientResultReadDto>(entity);
        }

        public async Task<PatientResultReadDto> UpdateAsync(int id, PatientResultUpdateDto dto)
        {
            var entity = await _uow.PatientResults.GetWithResultElementsAsync(id)
                ?? throw new NotFoundException("PatientResult", id);

            // Update AI output and summary
            entity.UpdateAIOutput(dto.AIClassifiedReport, dto.AISuggestion, dto.Summary);
            // Update notes and draft flag
            entity.Notes = dto.Notes ?? string.Empty;
            entity.IsDraft = dto.IsDraft;

            // Validate incoming elements
            foreach (var el in dto.ResultElements)
            {
                if (el.Id > 0)
                {
                    // ensure exists
                    var existing = entity.ResultElements.FirstOrDefault(re => re.Id == el.Id);
                    if (existing == null) throw new NotFoundException("PatientResultElement", el.Id);
                }
                else
                {
                    _ = await _uow.TestElements.GetByIdAsync(el.TestElementId) ?? throw new NotFoundException("TestElement", el.TestElementId);
                    _ = await _uow.LabTechnicians.GetByIdAsync(el.TechId) ?? throw new NotFoundException("LabTechnician", el.TechId);
                }
            }

            // Process elements: update existing, add new, remove missing
            var incomingIds = dto.ResultElements.Where(e => e.Id > 0).Select(e => e.Id).ToHashSet();
            var existingList = entity.ResultElements.ToList();

            // Remove elements not present in incoming list
            foreach (var existing in existingList)
            {
                if (!incomingIds.Contains(existing.Id))
                {
                    await _uow.PatientResultElements.SoftDeleteAsync(existing.Id);
                }
            }

            // Add or update incoming elements
            foreach (var el in dto.ResultElements)
            {
                if (el.Id > 0)
                {
                    var existing = existingList.First(re => re.Id == el.Id);
                    existing.UpdateValue(el.Value);
                    existing.TechId = el.TechId;
                    await _uow.PatientResultElements.UpdateAsync(existing);
                }
                else
                {
                    var created = new Domain.Entities.PatientResultElement(entity.Id, el.TestElementId, el.Value, el.TechId);
                    await _uow.PatientResultElements.AddAsync(created);
                }
            }

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
