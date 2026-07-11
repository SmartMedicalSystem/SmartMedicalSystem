using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface ILabTechnicianRepo : IGenericRepository<LabTechnician>
    {
        Task<LabTechnician?> GetByEmployeeIdAsync(string employeeId);

        Task<LabTechnician?> GetByNationalIdAsync(string nationalId);
    }
}
