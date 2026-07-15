using Application.Common;
using Application.DTOs.Doctor;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public DoctorService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        public async Task<DoctorReadDto> CreateAsync(DoctorCreateDto dto)
        {
            // Business rule: the department must exist before staffing a doctor to it.
            var department = await _uow.Departments.GetByIdAsync(dto.DepartmentId)
                ?? throw new NotFoundException("Department", dto.DepartmentId);

            var entity = new Domain.Entities.Doctor(dto.Name, dto.Specialization, dto.Contact, dto.Gender, dto.DepartmentId);
            await _uow.PersonGeneric.AddPerson(dto.NationalId.ToString(), entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<DoctorReadDto>(entity);
        }

        
        public async Task<DoctorReadDto> UpdateAsync(string ssn, DoctorUpdateDto dto)
        {
            var person = await _uow.PersonGeneric.FindBySSN(ssn)
                ?? throw new NotFoundException("Doctor", ssn);

            if (person is not Domain.Entities.Doctor entity)
                throw new NotFoundException("Doctor", ssn);

            entity.UpdateProfile(dto.Name, dto.Specialization, dto.Contact, dto.Gender, dto.Email, dto.MobileNumber, dto.Address);

            // Persist changes and update encrypted SSN if NationalId changed
            await _uow.PersonGeneric.UpdateSSNAsync(entity, dto.NationalId.ToString());

            return _mapper.Map<DoctorReadDto>(entity);
        }

        public async Task<DoctorReadDto> GetBySSNAsync(string ssn)
        {
            var entity = await _uow.PersonGeneric.FindBySSN(ssn) as Domain.Entities.Doctor
                ?? throw new NotFoundException("Doctor", ssn    );
            return _mapper.Map<DoctorReadDto>(entity);
        }

        public async Task<PaginatedResult<DoctorReadDto>> GetByDepartmentAsync(int departmentId, PaginationParams pagination)
        {
            var page = await _uow.Doctors.GetByDepartmentPaginatedAsync(departmentId, pagination);
            return PaginatedResult<DoctorReadDto>.Create(
                _mapper.Map<IEnumerable<DoctorReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task DeleteAsync(string ssn)
        {
            var person = await _uow.PersonGeneric.FindBySSN(ssn)
                ?? throw new NotFoundException("Doctor", ssn);

            await _uow.Doctors.SoftDeleteAsync(person.Id);
        }

        public async Task<PaginatedResult<DoctorReadDto>> GetAllAsync(PaginationParams pagination)
        {
            var page = await _uow.Doctors.GetAllActivePaginatedAsync(pagination);
            return PaginatedResult<DoctorReadDto>.Create(
                _mapper.Map<IEnumerable<DoctorReadDto>>(page.Items),
                page.TotalCount, pagination);
        }
    }
}
