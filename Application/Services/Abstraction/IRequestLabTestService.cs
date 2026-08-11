using Application.DTOs.RequestLabTests;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Services.Abstraction
{
    public interface IRequestLabTestService
    {
        Task<RequestLabTestReadDto> UpdateLabTestStatusAsync(int requestLabId, int labTestId, RequestLabTestStatus status);
        Task<IEnumerable<RequestLabTestSummaryDto>> GetPendingTestsAsync(int requestLabId);
        Task<IEnumerable<RequestLabTestSummaryDto>> GetCompletedTestsAsync(int requestLabId);
        Task<IEnumerable<RequestLabTestReadDto>> GetAllTestsForRequestAsync(int requestLabId);
    }
}
