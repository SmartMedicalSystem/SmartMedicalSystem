using Application.Common;
using Application.DTOs.Doctor;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
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

        public async Task<DoctorReadDto> CreateAsync(DoctorCreateDto dto)
        {
            // Business rule: the department must exist before staffing a doctor to it.
            var department = await _uow.Departments.GetByIdAsync(dto.DepartmentId)
                ?? throw new NotFoundException("Department", dto.DepartmentId);

            var entity = new Domain.Entities.Doctor(dto.Name, dto.Specialization, dto.Contact, dto.Gender, dto.DepartmentId);
            await _uow.Doctors.AddAsync(entity);
            return _mapper.Map<DoctorReadDto>(entity);
        }

        public async Task<DoctorReadDto> UpdateAsync(int id, DoctorUpdateDto dto)
        {
            var entity = await _uow.Doctors.GetByIdAsync(id)
                ?? throw new NotFoundException("Doctor", id);

            entity.UpdateProfile(dto.Name, dto.Specialization, dto.Contact, dto.Gender, dto.Email, dto.MobileNumber, dto.Address);
            await _uow.Doctors.UpdateAsync(entity);
            return _mapper.Map<DoctorReadDto>(entity);
        }

        public async Task<DoctorReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.Doctors.GetByIdAsync(id)
                ?? throw new NotFoundException("Doctor", id);
            return _mapper.Map<DoctorReadDto>(entity);
        }

        public async Task<PaginatedResult<DoctorReadDto>> GetByDepartmentAsync(int departmentId, PaginationParams pagination)
        {
            var page = await _uow.Doctors.GetByDepartmentPaginatedAsync(departmentId, pagination);
            return PaginatedResult<DoctorReadDto>.Create(
                _mapper.Map<IEnumerable<DoctorReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _uow.Doctors.ExistsAsync(id);
            if (!exists) throw new NotFoundException("Doctor", id);
            await _uow.Doctors.SoftDeleteAsync(id);
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
