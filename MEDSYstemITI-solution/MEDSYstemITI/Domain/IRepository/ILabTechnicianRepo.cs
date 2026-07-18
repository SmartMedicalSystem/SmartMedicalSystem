using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface ILabTechnicianRepo : IGenericRepository<LabTechnician>
    {
        //Task<LabTechnician?> GetByEmployeeIdAsync(string employeeId);

        Task<LabTechnician?> GetByNationalIdAsync(string nationalId);
        Task<PaginatedResult<LabTechnician>> SearchAsync(
         string? search,
         string? laboratory,
         EmploymentStatus? employmentStatus,
         WorkShift? workShift,
         DateOnly? joiningDate,
         PaginationParams pagination);
    }
}
