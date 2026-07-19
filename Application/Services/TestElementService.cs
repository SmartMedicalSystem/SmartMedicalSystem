using Application.Common;
using Application.DTOs.TestElement;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TestElementService : ITestElementService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public TestElementService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<TestElementReadDto> CreateAsync(TestElementCreateDto dto)
        {
            var entity = new Domain.Entities.TestElement(dto.ElementName, dto.Unit, dto.NormalMin, dto.NormalMax);
            await _uow.TestElements.AddAsync(entity);
            return _mapper.Map<TestElementReadDto>(entity);
        }

        public async Task<TestElementReadDto> UpdateAsync(int id, TestElementUpdateDto dto)
        {
            var entity = await _uow.TestElements.GetByIdAsync(id)
                ?? throw new NotFoundException("TestElement", id);

            entity.UpdateRange(dto.NormalMin, dto.NormalMax);
            await _uow.TestElements.UpdateAsync(entity);
            return _mapper.Map<TestElementReadDto>(entity);
        }

        public async Task<TestElementReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.TestElements.GetByIdAsync(id)
                ?? throw new NotFoundException("TestElement", id);
            return _mapper.Map<TestElementReadDto>(entity);
        }

        public async Task<PaginatedResult<TestElementReadDto>> GetAllAsync(PaginationParams pagination)
        {
            var page = await _uow.TestElements.GetAllActivePaginatedAsync(pagination);
            return PaginatedResult<TestElementReadDto>.Create(
                _mapper.Map<IEnumerable<TestElementReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _uow.TestElements.ExistsAsync(id);
            if (!exists) throw new NotFoundException("TestElement", id);
            await _uow.TestElements.SoftDeleteAsync(id);
        }
    }
}
