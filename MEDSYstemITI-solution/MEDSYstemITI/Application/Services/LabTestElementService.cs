using Application.Common;
using Application.DTOs.LabTestElement;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LabTestElementService : ILabTestElementService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public LabTestElementService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<LabTestElementReadDto> AddElementToTestAsync(LabTestElementCreateDto dto)
        {
            _ = await _uow.LabTests.GetByIdAsync(dto.LabTestId) ?? throw new NotFoundException("LabTest", dto.LabTestId);
            _ = await _uow.TestElements.GetByIdAsync(dto.TestElementId) ?? throw new NotFoundException("TestElement", dto.TestElementId);

            var alreadyLinked = await _uow.LabTestElements.ExistsAsync(dto.LabTestId, dto.TestElementId);
            if (alreadyLinked)
                throw new System.ArgumentException("This test element is already linked to this lab test.");

            var entity = new Domain.Entities.LabTestElement(dto.LabTestId, dto.TestElementId);
            await _uow.LabTestElements.AddAsync(entity);
            return _mapper.Map<LabTestElementReadDto>(entity);
        }

        public async Task<IEnumerable<LabTestElementReadDto>> GetByLabTestAsync(int labTestId)
        {
            var items = await _uow.LabTestElements.GetByLabTestAsync(labTestId);
            return _mapper.Map<IEnumerable<LabTestElementReadDto>>(items);
        }

        public async Task RemoveElementFromTestAsync(int labTestId, int testElementId)
        {
            var exists = await _uow.LabTestElements.ExistsAsync(labTestId, testElementId);
            if (!exists) throw new NotFoundException("LabTestElement", $"{labTestId}-{testElementId}");
            await _uow.LabTestElements.DeleteAsync(labTestId, testElementId);
        }
    }
}
