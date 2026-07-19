using Domain.Entities;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IDoctorRepo : IGenericRepository<Doctor>
    {
        // Entity-specific reads
        Task<Doctor?> GetByNameAsync(string name);
        Task<IEnumerable<Doctor>> GetByDepartmentAsync(int departmentId);
        Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization);

        // Entity-specific reads with Pagination
        Task<PaginatedResult<Doctor>> GetByDepartmentPaginatedAsync(int departmentId, PaginationParams pagination);
        Task<PaginatedResult<Doctor>> GetBySpecializationPaginatedAsync(string specialization, PaginationParams pagination);
    }
}
