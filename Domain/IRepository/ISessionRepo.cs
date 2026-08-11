using Domain.Entities;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface ISessionRepo : IGenericRepository<Session>
    {
        // Entity-specific reads
        Task<IEnumerable<Session>> GetByPatientAsync(int patientId);
        Task<IEnumerable<Session>> GetByDoctorAsync(int doctorId);
        Task<IEnumerable<Session>> GetByDepartmentAsync(int departmentId);
        Task<Session?> GetWithDetailsAsync(int id);

        // Entity-specific reads with Pagination
        Task<PaginatedResult<Session>> GetByPatientPaginatedAsync(int patientId, PaginationParams pagination);
        Task<PaginatedResult<Session>> GetByDoctorPaginatedAsync(int doctorId, PaginationParams pagination);
        Task<PaginatedResult<Session>> GetByDepartmentPaginatedAsync(int departmentId, PaginationParams pagination);
    }
}
