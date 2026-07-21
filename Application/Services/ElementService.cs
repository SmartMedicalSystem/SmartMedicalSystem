using Application.Common;
using Application.DTOs.Element;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ElementService : IElementService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ElementService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ElementReadDto> CreateAsync(ElementCreateDto dto)
        {
            var entity = new Domain.Entities.Element(dto.Name, dto.Unit, dto.ReferenceRange, dto.Description);
            await _uow.Elements.AddAsync(entity);
            return _mapper.Map<ElementReadDto>(entity);
        }

        public async Task<ElementReadDto> UpdateAsync(int id, ElementUpdateDto dto)
        {
            var entity = await _uow.Elements.GetByIdAsync(id)
                ?? throw new NotFoundException("Element", id);

            entity.UpdateDetails(dto.Name, dto.Unit, dto.ReferenceRange, dto.Description);
            await _uow.Elements.UpdateAsync(entity);
            return _mapper.Map<ElementReadDto>(entity);
        }

        public async Task<ElementReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.Elements.GetByIdAsync(id)
                ?? throw new NotFoundException("Element", id);
            return _mapper.Map<ElementReadDto>(entity);
        }

        public async Task<PaginatedResult<ElementReadDto>> GetAllAsync(PaginationParams pagination)
        {
            var page = await _uow.Elements.GetAllActivePaginatedAsync(pagination);
            return PaginatedResult<ElementReadDto>.Create(
                _mapper.Map<IEnumerable<ElementReadDto>>(page.Items),
                page.TotalCount,
                pagination);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _uow.Elements.ExistsAsync(id);
            if (!exists) throw new NotFoundException("Element", id);
            await _uow.Elements.SoftDeleteAsync(id);
        }
    }
}
