using Application.Common;
using Application.DTOs.LabTechnician;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using Domain.Models;

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
            var entity = new Domain.Entities.LabTechnician(dto.Name, dto.Contact, dto.NationalId);
            await _uow.PersonGeneric.AddPerson(dto.NationalId.ToString(), entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<LabTechnicianReadDto> UpdateAsync(string ssn, LabTechnicianUpdateDto dto)
        {
            var person = await _uow.PersonGeneric.FindBySSN(ssn)
                ?? throw new NotFoundException("LabTechnician", ssn);

            if (person is not Domain.Entities.LabTechnician entity)
                throw new NotFoundException("LabTechnician", ssn);

            entity.UpdateProfile(dto.Name, dto.Contact);
            await _uow.LabTechnicians.UpdateAsync(entity);

            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<LabTechnicianReadDto> GetBySSNAsync(string ssn)
        {
            var entity = await _uow.PersonGeneric.FindBySSN(ssn) 
                ?? throw new NotFoundException("LabTechnician", ssn);
            return _mapper.Map<LabTechnicianReadDto>(entity);
        }

        public async Task<PaginatedResult<LabTechnicianReadDto>> GetAllAsync(LabTechnicianFilterDto filter)
        {
            var page = await _uow.LabTechnicians.SearchAsync(
                filter.Search,
                filter.Laboratory,
                filter.EmploymentStatus,
                filter.WorkShift,
                filter.JoiningDate,
                filter);

            return PaginatedResult<LabTechnicianReadDto>.Create(
                _mapper.Map<IEnumerable<LabTechnicianReadDto>>(page.Items),
                page.TotalCount,
                filter);
        }

        public async Task DeleteAsync(string ssn)
        {
            var person = await _uow.PersonGeneric.FindBySSN(ssn)
                ?? throw new NotFoundException("LabTechnician", ssn);

            await _uow.LabTechnicians.SoftDeleteAsync(person.Id);
        }



      
    }
}