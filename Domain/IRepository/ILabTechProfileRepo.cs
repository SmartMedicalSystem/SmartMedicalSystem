using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface ILabTechProfileRepo : IGenericRepository<LabTechProfile>
    {
        /// <summary>Fetches the profile belonging to the currently logged-in technician (self-service page).</summary>
        Task<LabTechProfile?> GetByUserIdAsync(int userId);

        /// <summary>Used on Create to make sure a technician doesn't get two profiles.</summary>
        Task<bool> ExistsForUserAsync(int userId);

        /// <summary>Used on Create to make sure Employee IDs stay unique.</summary>
        Task<bool> EmployeeIdExistsAsync(string employeeId);
    }
}