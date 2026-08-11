using Domain.Entities;
using Domain.Filters;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IPatientRepo : IGenericRepository<Patient>
    {
        Task<IEnumerable<Patient>> GetAllWithSessionsAsync();

        Task<PaginatedResult<Patient>> GetFilteredPaginatedAsync(
            PatientFilterParams filter);

        Task<PaginatedResult<Patient>> GetAllWithSessionsPaginatedAsync(
            PaginationParams pagination);
    }
}