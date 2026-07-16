using Application.Common;
using Application.DTOs.Department;
using Application.DTOs.Doctor;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // ============================================================
        // 1. CREATE
        // ============================================================
        public async Task<DepartmentReadDto> CreateAsync(DepartmentCreateDto dto)
        {
            // Business rule: department names must be unique.
            var isUnique = await _uow.Departments.IsDepartmentNameUniqueAsync(dto.Name);
            if (!isUnique)
                throw new ArgumentException($"A department named '{dto.Name}' already exists.");

            var entity = new Domain.Entities.Department(
                dto.Name,
                dto.HeadDoctor,
                dto.FloorNumber,
                dto.Status
            );

            // Assign Head Doctor if provided
            if (dto.HeadDoctorId.HasValue)
            {
                var doctor = await _uow.Doctors.GetByIdAsync(dto.HeadDoctorId.Value);
                if (doctor == null)
                    throw new NotFoundException("Doctor", dto.HeadDoctorId.Value);

                if (doctor.DepartmentId != entity.Id)
                    throw new ArgumentException("Head doctor must be a staff member of the same department.");

                entity.AssignHeadDoctor(dto.HeadDoctorId.Value, dto.HeadDoctor);
            }

            await _uow.Departments.AddAsync(entity);
            await _uow.SaveChangesAsync();

            return _mapper.Map<DepartmentReadDto>(entity);
        }

        // ============================================================
        // 2. UPDATE
        // ============================================================
        public async Task<DepartmentReadDto> UpdateAsync(int id, DepartmentUpdateDto dto)
        {
            var entity = await _uow.Departments.GetDepartmentWithDoctorsAsync(id)
                ?? throw new NotFoundException("Department", id);

            // Check unique name (excluding current)
            var isUnique = await _uow.Departments.IsDepartmentNameUniqueAsync(dto.Name, id);
            if (!isUnique)
                throw new ArgumentException($"A department named '{dto.Name}' already exists.");

            entity.UpdateDetails(
                dto.Name,
                dto.HeadDoctor,
                dto.FloorNumber,
                dto.Status
            );

            if (dto.HeadDoctorId.HasValue)
            {
                var doctor = await _uow.Doctors.GetByIdAsync(dto.HeadDoctorId.Value);
                if (doctor == null)
                    throw new NotFoundException("Doctor", dto.HeadDoctorId.Value);

                if (doctor.DepartmentId != entity.Id)
                    throw new ArgumentException("Head doctor must be a staff member of the same department.");

                entity.AssignHeadDoctor(dto.HeadDoctorId.Value, dto.HeadDoctor);
            }
            else
            {
                entity.RemoveHeadDoctor();
            }

           await _uow.Departments.UpdateAsync(entity);
            await _uow.SaveChangesAsync();

            return _mapper.Map<DepartmentReadDto>(entity);
        }

        // ============================================================
        // 3. GET BY ID
        // ============================================================
        public async Task<DepartmentReadDto> GetByIdAsync(int id)
        {
            var entity = await _uow.Departments.GetDepartmentWithDoctorsAsync(id)
                ?? throw new NotFoundException("Department", id);

            var dto = _mapper.Map<DepartmentReadDto>(entity);

            dto.Doctors = entity.Doctors?.Select(d => new DoctorAtDepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Specialization = d.Specialization
            }).ToList() ?? new();

            return dto;
        }

        // ============================================================
        // 4. GET ALL (PAGINATED)
        // ============================================================
        public async Task<PaginatedResult<DepartmentReadDto>> GetAllAsync(
    PaginationParams pagination,
    string? searchTerm = null,
    string? statusFilter = null,
    string? creationDateFilter = null,
    string? managerFilter = null)
        {
            var page = await _uow.Departments.GetAllPaginatedAsync(
       pagination,
       searchTerm,
       statusFilter,
       creationDateFilter,
       managerFilter
   );

            var dtos = page.Items.Select(entity => new DepartmentReadDto
            {
                Id = entity.Id,
                Name = entity.Name,
                FloorNumber = entity.FloorNumber,
                HeadDoctor = entity.HeadDoctor,
                HeadDoctorId = entity.HeadDoctorId,
                Status = entity.Status,
                DoctorCount = entity.Doctors?.Count ?? 0,
                CreatedAt = entity.CreatedAt,
                Doctors = entity.Doctors?.Select(d => new DoctorAtDepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Specialization = d.Specialization
                }).ToList() ?? new()
            }).ToList();

            return PaginatedResult<DepartmentReadDto>.Create(dtos, page.TotalCount, pagination);
        }




        // ============================================================
        // 5. GET ACTIVE DEPARTMENTS
        // ============================================================
        public async Task<IEnumerable<DepartmentReadDto>> GetActiveDepartmentsAsync()
        {
            var entities = await _uow.Departments.GetActiveDepartmentsAsync();

            return entities.Select(entity => new DepartmentReadDto
            {
                Id = entity.Id,
                Name = entity.Name,
                HeadDoctor = entity.HeadDoctor,
                Status = entity.Status
            });
        }

        // ============================================================
        // 6. CHECK UNIQUE NAME
        // ============================================================
        public async Task<bool> IsDepartmentNameUniqueAsync(string name, int? excludeId = null)
        {
            return await _uow.Departments.IsDepartmentNameUniqueAsync(name, excludeId);
        }

        // ============================================================
        // 7. ASSIGN HEAD DOCTOR
        // ============================================================
        public async Task AssignHeadDoctorAsync(int departmentId, int doctorId)
        {
            var department = await _uow.Departments.GetDepartmentWithDoctorsAsync(departmentId)
                ?? throw new NotFoundException("Department", departmentId);

            var doctor = await _uow.Doctors.GetByIdAsync(doctorId)
                ?? throw new NotFoundException("Doctor", doctorId);

            if (doctor.DepartmentId != departmentId)
                throw new ArgumentException("Head doctor must be a staff member of the same department.");

            department.AssignHeadDoctor(doctorId, doctor.Name);
           await _uow.Departments.UpdateAsync(department);
            await _uow.SaveChangesAsync();
        }

        // ============================================================
        // 8. DELETE (SOFT DELETE)
        // ============================================================
        public async Task DeleteAsync(int id)
        {
            var exists = await _uow.Departments.ExistsAsync(id);
            if (!exists) throw new NotFoundException("Department", id);

            await _uow.Departments.SoftDeleteAsync(id);
            await _uow.SaveChangesAsync();
        }
    }
}