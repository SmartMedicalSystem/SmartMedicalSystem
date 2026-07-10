using Application.DTOs.Doctor;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IDoctorService
    {
        Task<DoctorReadDto> CreateAsync(DoctorCreateDto dto);
        Task<DoctorReadDto> UpdateAsync(int id, DoctorUpdateDto dto);
        Task<DoctorReadDto> GetByIdAsync(int id);
        Task<PaginatedResult<DoctorReadDto>> GetByDepartmentAsync(int departmentId, PaginationParams pagination);
        Task DeleteAsync(int id);
    }
}
