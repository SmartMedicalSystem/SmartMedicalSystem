using Application.DTOs.Department;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IDepartmentService
    {
        Task<DepartmentReadDto> CreateAsync(DepartmentCreateDto dto);
        Task<DepartmentReadDto> UpdateAsync(int id, DepartmentUpdateDto dto);
        Task<DepartmentReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<DepartmentReadDto>> GetAllAsync(PaginationParams pagination);
        Task AssignHeadDoctorAsync(int departmentId, int doctorId);
        Task DeleteAsync(int id);
    }
}
