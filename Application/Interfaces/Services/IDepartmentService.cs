using Application.DTOs.Department;

namespace Application.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();

        Task<DepartmentDto?> GetByIdAsync(int id);

        Task CreateAsync(CreateDepartmentDto dto);

        Task UpdateAsync(int id, UpdateDepartmentDto dto);

        Task DeleteAsync(int id);
    }
}