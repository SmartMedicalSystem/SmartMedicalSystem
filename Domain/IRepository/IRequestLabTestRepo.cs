using Domain.Entities;
using Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IRequestLabTestRepo
    {
        Task<RequestLabTest?> GetByIdsAsync(int requestLabId, int labTestId);
        Task<IEnumerable<RequestLabTest>> GetPendingTestsAsync(int requestLabId);
        Task<IEnumerable<RequestLabTest>> GetCompletedTestsAsync(int requestLabId);
        Task<IEnumerable<RequestLabTest>> GetByRequestLabAsync(int requestLabId);
        Task UpdateStatusAsync(int requestLabId, int labTestId, RequestLabTestStatus status);
    }
}
