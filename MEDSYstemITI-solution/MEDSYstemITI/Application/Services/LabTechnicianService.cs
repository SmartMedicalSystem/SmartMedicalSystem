using Application.Common;
using Application.DTOs.LabTechnician;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LabTechnicianService : ILabTechnicianService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public LabTechnicianService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<LabTechnicianReadDto> CreateAsync(LabTechnicianCreateDto dto)
        {
            var entity = new Domain.Entities.LabTechnician(dto.Name, dto.Contact);
            await _uow.LabTechnicians.AddAsync(entity);
            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<LabTechnicianReadDto> UpdateAsync(int id, LabTechnicianUpdateDto dto)
        {
            var entity = await _uow.LabTechnicians.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTechnician", id);

            entity.UpdateProfile(dto.Name, dto.Contact);
            await _uow.LabTechnicians.UpdateAsync(entity);
            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<LabTechnicianReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.LabTechnicians.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTechnician", id);
            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<PaginatedResult<LabTechnicianReadDto>> GetAllAsync(PaginationParams pagination)
        {
            var page = await _uow.LabTechnicians.GetAllActivePaginatedAsync(pagination);
            return PaginatedResult<LabTechnicianReadDto>.Create(
                _mapper.Map<IEnumerable<LabTechnicianReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _uow.LabTechnicians.ExistsAsync(id);
            if (!exists) throw new NotFoundException("LabTechnician", id);
            await _uow.LabTechnicians.SoftDeleteAsync(id);
        }
    }
}
