using Application.Common;
using Application.DTOs.LabTest;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LabTestService : ILabTestService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public LabTestService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<LabTestReadDto> CreateAsync(LabTestCreateDto dto)
        {
            var existing = await _uow.LabTests.GetByNameAsync(dto.TestName);
            if (existing is not null)
                throw new System.ArgumentException($"A lab test named '{dto.TestName}' already exists.");

            var entity = new Domain.Entities.LabTest(dto.TestName, dto.Description);
            await _uow.LabTests.AddAsync(entity);
            return _mapper.Map<LabTestReadDto>(entity);
        }

        public async Task<LabTestReadDto> UpdateAsync(int id, LabTestUpdateDto dto)
        {
            var entity = await _uow.LabTests.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTest", id);

            entity.UpdateDetails(dto.TestName, dto.Description);
            await _uow.LabTests.UpdateAsync(entity);
            return _mapper.Map<LabTestReadDto>(entity);
        }

        public async Task<LabTestReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.LabTests.GetByIdAsync(id)
                ?? throw new NotFoundException("LabTest", id);
            return _mapper.Map<LabTestReadDto>(entity);
        }

        public async Task<PaginatedResult<LabTestReadDto>> GetAllAsync(PaginationParams pagination)
        {
            var page = await _uow.LabTests.GetAllActivePaginatedAsync(pagination);
            return PaginatedResult<LabTestReadDto>.Create(
                _mapper.Map<IEnumerable<LabTestReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _uow.LabTests.ExistsAsync(id);
            if (!exists) throw new NotFoundException("LabTest", id);
            await _uow.LabTests.SoftDeleteAsync(id);
        }
    }
}
