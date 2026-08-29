    using Application.Common;
using Application.DTOs.Department;
using Application.DTOs.Doctor;
using Application.Services.Abstraction;
using Domain.IRepository;
using Domain.Models;

namespace Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _uow;

        public DepartmentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ============================================================
        // Helper Methods - Manual Mapping
        // ============================================================

        private static DepartmentReadDto MapToReadDto(
            Domain.Entities.Department entity)
        {
            return new DepartmentReadDto
            {
                Id = entity.Id,
                Name = entity.Name,
                FloorNumber = entity.FloorNumber,
                HeadDoctor = entity.HeadDoctor,
                HeadDoctorId = entity.HeadDoctorId,
                Status = entity.Status,
                DoctorCount = entity.Doctors?.Count ?? 0,
                CreatedAt = entity.CreatedAt,

                Doctors = entity.Doctors?
                    .Select(d => new DoctorAtDepartmentDto
                    {
                        Id = d.Id,
                        Name = $"{d.FirstName} {d.LastName}".Trim(),
                        Specialization = d.Specialization
                    })
                    .ToList() ?? new List<DoctorAtDepartmentDto>()
            };
        }

        private static DepartmentReadDto MapToBasicReadDto(
            Domain.Entities.Department entity)
        {
            return new DepartmentReadDto
            {
                Id = entity.Id,
                Name = entity.Name,
                FloorNumber = entity.FloorNumber,
                HeadDoctor = entity.HeadDoctor,
                HeadDoctorId = entity.HeadDoctorId,
                Status = entity.Status,
                DoctorCount = entity.Doctors?.Count ?? 0,
                CreatedAt = entity.CreatedAt,

                Doctors = new List<DoctorAtDepartmentDto>()
            };
        }


        // ============================================================
        // 1. CREATE
        // ============================================================

        public async Task<DepartmentReadDto> CreateAsync(
            DepartmentCreateDto dto)
        {
            // Check unique name
            var isUnique =
                await _uow.Departments
                    .IsDepartmentNameUniqueAsync(dto.Name);

            if (!isUnique)
            {
                throw new ArgumentException(
                    $"A department named '{dto.Name}' already exists.");
            }

            // Create Department
            var entity = new Domain.Entities.Department(
                dto.Name,
                dto.HeadDoctor,
                dto.FloorNumber,
                dto.Status
            );

            await _uow.Departments.AddAsync(entity);
            await _uow.SaveChangesAsync();

            // Assign Head Doctor if selected
            if (dto.HeadDoctorId.HasValue)
            {
                var doctor =
                    await _uow.Doctors
                        .GetByIdAsync(dto.HeadDoctorId.Value);

                if (doctor == null)
                {
                    throw new NotFoundException(
                        "Doctor",
                        dto.HeadDoctorId.Value);
                }

                // Doctor must belong to this department
                doctor.ReassignDepartment(entity.Id);

                entity.AssignHeadDoctor(
                    doctor.Id,
                    $"{doctor.FirstName} {doctor.LastName}".Trim()
                );

                await _uow.SaveChangesAsync();
            }

            return MapToReadDto(entity);
        }


        // ============================================================
        // 2. UPDATE
        // ============================================================

        public async Task<DepartmentReadDto> UpdateAsync(
            int id,
            DepartmentUpdateDto dto)
        {
            var entity =
                await _uow.Departments
                    .GetDepartmentWithDoctorsAsync(id)
                ?? throw new NotFoundException(
                    "Department",
                    id);

            // Check unique name
            var isUnique =
                await _uow.Departments
                    .IsDepartmentNameUniqueAsync(dto.Name, id);

            if (!isUnique)
            {
                throw new ArgumentException(
                    $"A department named '{dto.Name}' already exists.");
            }

            // Update basic data
            entity.UpdateDetails(
                dto.Name,
                dto.HeadDoctor,
                dto.FloorNumber,
                dto.Status
            );

            // Assign / Remove Head Doctor
            if (dto.HeadDoctorId.HasValue)
            {
                var doctor =
                    await _uow.Doctors
                        .GetByIdAsync(dto.HeadDoctorId.Value);

                if (doctor == null)
                {
                    throw new NotFoundException(
                        "Doctor",
                        dto.HeadDoctorId.Value);
                }

                // Important Business Rule
                if (doctor.DepartmentId != entity.Id)
                {
                    throw new ArgumentException(
                        "Head doctor must be a staff member of the same department.");
                }

                entity.AssignHeadDoctor(
                    doctor.Id,
                    $"{doctor.FirstName} {doctor.LastName}".Trim()
                );
            }
            else
            {
                entity.RemoveHeadDoctor();
            }

            await _uow.Departments.UpdateAsync(entity);
            await _uow.SaveChangesAsync();

            return MapToReadDto(entity);
        }


        // ============================================================
        // 3. GET BY ID
        // ============================================================

        public async Task<DepartmentReadDto> GetByIdAsync(int id)
        {
            var entity =
                await _uow.Departments
                    .GetDepartmentWithDoctorsAsync(id)
                ?? throw new NotFoundException(
                    "Department",
                    id);

            return MapToReadDto(entity);
        }


        // ============================================================
        // 4. GET ALL
        // ============================================================

        public async Task<PaginatedResult<DepartmentReadDto>> GetAllAsync(
            PaginationParams pagination,
            string? searchTerm = null,
            string? statusFilter = null,
            string? creationDateFilter = null,
            string? managerFilter = null)
        {
            var page =
                await _uow.Departments
                    .GetAllPaginatedAsync(
                        pagination,
                        searchTerm,
                        statusFilter,
                        creationDateFilter,
                        managerFilter
                    );

            var dtos = page.Items
                .Select(MapToBasicReadDto)
                .ToList();

            return PaginatedResult<DepartmentReadDto>.Create(
                dtos,
                page.TotalCount,
                pagination
            );
        }


        // ============================================================
        // 5. GET ACTIVE DEPARTMENTS
        // ============================================================

        public async Task<IEnumerable<DepartmentReadDto>>
            GetActiveDepartmentsAsync()
        {
            var entities =
                await _uow.Departments
                    .GetActiveDepartmentsAsync();

            return entities.Select(entity => new DepartmentReadDto
            {
                Id = entity.Id,
                Name = entity.Name,
                HeadDoctor = entity.HeadDoctor,
                HeadDoctorId = entity.HeadDoctorId,
                Status = entity.Status,
                DoctorCount = entity.Doctors?.Count ?? 0,
                CreatedAt = entity.CreatedAt,
                Doctors = new List<DoctorAtDepartmentDto>()
            });
        }


        // ============================================================
        // 6. CHECK UNIQUE NAME
        // ============================================================

        public async Task<bool> IsDepartmentNameUniqueAsync(
            string name,
            int? excludeId = null)
        {
            return await _uow.Departments
                .IsDepartmentNameUniqueAsync(
                    name,
                    excludeId
                );
        }


        // ============================================================
        // 7. ASSIGN HEAD DOCTOR
        // ============================================================

        public async Task AssignHeadDoctorAsync(
            int departmentId,
            int doctorId)
        {
            var department =
                await _uow.Departments
                    .GetDepartmentWithDoctorsAsync(
                        departmentId
                    )
                ?? throw new NotFoundException(
                    "Department",
                    departmentId);

            var doctor =
                await _uow.Doctors
                    .GetByIdAsync(doctorId)
                ?? throw new NotFoundException(
                    "Doctor",
                    doctorId);

            if (doctor.DepartmentId != departmentId)
            {
                throw new ArgumentException(
                    "Head doctor must be a staff member of the same department.");
            }

            department.AssignHeadDoctor(
                doctor.Id,
                $"{doctor.FirstName} {doctor.LastName}".Trim()
            );

            await _uow.Departments.UpdateAsync(department);
            await _uow.SaveChangesAsync();
        }


        // ============================================================
        // 8. DELETE
        // ============================================================

        public async Task DeleteAsync(int id)
        {
            var exists =
                await _uow.Departments
                    .ExistsAsync(id);

            if (!exists)
            {
                throw new NotFoundException(
                    "Department",
                    id);
            }

            await _uow.Departments
                .SoftDeleteAsync(id);

            await _uow.SaveChangesAsync();
        }
    }
}