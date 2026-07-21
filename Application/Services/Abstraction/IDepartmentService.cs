using Application.DTOs.Department;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IDepartmentService
    {
        Task<DepartmentReadDto> CreateAsync(DepartmentCreateDto dto);
        Task<DepartmentReadDto> UpdateAsync(int id, DepartmentUpdateDto dto);
        Task<DepartmentReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<DepartmentReadDto>> GetAllAsync(
            PaginationParams pagination,
            string? searchTerm = null,
            string? statusFilter = null,
            string? creationDateFilter = null,
            string? managerFilter = null);


        Task<IEnumerable<DepartmentReadDto>> GetActiveDepartmentsAsync();
        Task<bool> IsDepartmentNameUniqueAsync(string name, int? excludeId = null);
        Task AssignHeadDoctorAsync(int departmentId, int doctorId);
        Task DeleteAsync(int id);
    }
}