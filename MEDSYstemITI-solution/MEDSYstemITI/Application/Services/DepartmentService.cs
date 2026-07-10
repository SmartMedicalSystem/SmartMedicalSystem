using Application.Common;
using Application.DTOs.Department;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<DepartmentReadDto> CreateAsync(DepartmentCreateDto dto)
        {
            // Business rule: department names must be unique.
            var existing = await _uow.Departments.GetByNameAsync(dto.Name);
            if (existing is not null)
                throw new System.ArgumentException($"A department named '{dto.Name}' already exists.");

            var entity = new Domain.Entities.Department(dto.Name, dto.DepartmentMangager);
            await _uow.Departments.AddAsync(entity);
            return _mapper.Map<DepartmentReadDto>(entity);
        }

        public async Task<DepartmentReadDto> UpdateAsync(int id, DepartmentUpdateDto dto)
        {
            var entity = await _uow.Departments.GetByIdAsync(id)
                ?? throw new NotFoundException("Department", id);

            entity.UpdateDetails(dto.Name, dto.DepartmentMangager);
            await _uow.Departments.UpdateAsync(entity);
            return _mapper.Map<DepartmentReadDto>(entity);
        }

        public async Task<DepartmentReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.Departments.GetByIdAsync(id)
                ?? throw new NotFoundException("Department", id);
            return _mapper.Map<DepartmentReadDto>(entity);
        }

        public async Task<PaginatedResult<DepartmentReadDto>> GetAllAsync(PaginationParams pagination)
        {
            var page = await _uow.Departments.GetAllActivePaginatedAsync(pagination);
            return PaginatedResult<DepartmentReadDto>.Create(
                _mapper.Map<System.Collections.Generic.IEnumerable<DepartmentReadDto>>(page.Items),
                page.TotalCount, pagination);
        }

        public async Task AssignHeadDoctorAsync(int departmentId, int doctorId)
        {
            var department = await _uow.Departments.GetByIdAsync(departmentId)
                ?? throw new NotFoundException("Department", departmentId);

            // Business rule: the assigned doctor must exist and belong to this department.
            var doctor = await _uow.Doctors.GetByIdAsync(doctorId)
                ?? throw new NotFoundException("Doctor", doctorId);
            if (doctor.DepartmentId != departmentId)
                throw new System.ArgumentException("Head doctor must be a staff member of the same department.");

            department.AssignHeadDoctor(doctorId);
            await _uow.Departments.UpdateAsync(department);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _uow.Departments.ExistsAsync(id);
            if (!exists) throw new NotFoundException("Department", id);
            await _uow.Departments.SoftDeleteAsync(id);
        }
    }
}
