using Application.Common;
using Application.DTOs.PatientResultElement;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PatientResultElementService : IPatientResultElementService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PatientResultElementService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PatientResultElementReadDto> CreateAsync(PatientResultElementCreateDto dto)
        {
            _ = await _uow.PatientResults.GetByIdAsync(dto.PatientResultId)
                ?? throw new NotFoundException("PatientResult", dto.PatientResultId);
            _ = await _uow.TestElements.GetByIdAsync(dto.TestElementId)
                ?? throw new NotFoundException("TestElement", dto.TestElementId);
            _ = await _uow.LabTechnicians.GetByIdAsync(dto.TechId)
                ?? throw new NotFoundException("LabTechnician", dto.TechId);

            var entity = new Domain.Entities.PatientResultElement(
                dto.PatientResultId, dto.TestElementId, dto.Value, dto.TechId);

            await _uow.PatientResultElements.AddAsync(entity);
            var result = _mapper.Map<PatientResultElementReadDto>(entity);
            return result;
        }

        public async Task<PatientResultElementReadDto> UpdateAsync(int id, PatientResultElementUpdateDto dto)
        {
            var entity = await _uow.PatientResultElements.GetByIdAsync(id)
                ?? throw new NotFoundException("PatientResultElement", id);

            entity.UpdateValue(dto.Value);
            await _uow.PatientResultElements.UpdateAsync(entity);
            return _mapper.Map<PatientResultElementReadDto>(entity);
        }

        public async Task<PaginatedResult<PatientResultElementReadDto>> GetByPatientResultAsync(int patientResultId, PaginationParams pagination)
        {
            var page = await _uow.PatientResultElements.GetByPatientResultPaginatedAsync(patientResultId, pagination);
            return PaginatedResult<PatientResultElementReadDto>.Create(
                _mapper.Map<IEnumerable<PatientResultElementReadDto>>(page.Items),
                page.TotalCount, pagination);
        }
    }
}
