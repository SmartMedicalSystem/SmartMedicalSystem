using Application.DTOs.Doctor;
using Domain.Models;
using System.Threading.Tasks;

namespace Application.Services.Abstraction
{
    public interface IDoctorService
    {
        Task<DoctorReadDto> CreateAsync(DoctorCreateDto dto);
        Task<DoctorReadDto> UpdateAsync(string ssn, DoctorUpdateDto dto);
        Task<DoctorReadDto> GetBySSNAsync(string ssn);
        Task<PaginatedResult<DoctorReadDto>> GetByDepartmentAsync(int departmentId, PaginationParams pagination);
        
        Task<PaginatedResult<DoctorReadDto>> GetAllAsync(PaginationParams pagination);
        Task DeleteAsync(string ssn);
    }
}
