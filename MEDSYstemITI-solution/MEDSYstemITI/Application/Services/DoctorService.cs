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
        private readonly Domain.IRepository.IPersonGenericRepo _personRepo;
        private readonly IMapper _mapper;

        public DoctorService(IUnitOfWork uow, Domain.IRepository.IPersonGenericRepo personRepo, IMapper mapper)
        {
            _uow = uow;
            _personRepo = personRepo;
            _mapper = mapper;
        }

        // Compatibility overloads that accept entity id instead of SSN
        public async Task<DoctorReadDto> UpdateAsync(int id, DoctorUpdateDto dto)
        {
            var entity = await _uow.Doctors.GetByIdAsync(id)
                ?? throw new NotFoundException("Doctor", id);

            entity.UpdateProfile(dto.Name, dto.Specialization, dto.Contact, dto.Gender, dto.Email, dto.MobileNumber, dto.Address);
            await _uow.Doctors.UpdateAsync(entity);
            return _mapper.Map<DoctorReadDto>(entity);
        }

        // (int-based members implemented above)

        public async Task<DoctorReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.Doctors.GetByIdAsync(id)
                ?? throw new NotFoundException("Doctor", id);
            return _mapper.Map<DoctorReadDto>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _uow.Doctors.SoftDeleteAsync(id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<DoctorReadDto> CreateAsync(DoctorCreateDto dto)
        {
            // Business rule: the department must exist before staffing a doctor to it.
            var department = await _uow.Departments.GetByIdAsync(dto.DepartmentId)
                ?? throw new NotFoundException("Department", dto.DepartmentId);

            var entity = new Domain.Entities.Doctor(dto.Name, dto.Specialization, dto.Contact, dto.Gender, dto.DepartmentId);
            await _personRepo.AddPerson(dto.NationalId.ToString(), entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<DoctorReadDto>(entity);
        }

        
        public async Task<DoctorReadDto> UpdateAsync(string ssn, DoctorUpdateDto dto)
        {
            var person = await _personRepo.FindBySSN(ssn)
                ?? throw new NotFoundException("Doctor", ssn);

            if (person is not Domain.Entities.Doctor entity)
                throw new NotFoundException("Doctor", ssn);

            entity.UpdateProfile(dto.Name, dto.Specialization, dto.Contact, dto.Gender, dto.Email, dto.MobileNumber, dto.Address);

            // Persist changes and update encrypted SSN if NationalId changed
            await _personRepo.UpdateSSNAsync(entity, dto.NationalId.ToString());

            return _mapper.Map<DoctorReadDto>(entity);
        }

        public async Task<DoctorReadDto> GetBySSNAsync(string ssn)
        {
            var entity = await _personRepo.FindBySSN(ssn) as Domain.Entities.Doctor
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
            var person = await _personRepo.FindBySSN(ssn)
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
