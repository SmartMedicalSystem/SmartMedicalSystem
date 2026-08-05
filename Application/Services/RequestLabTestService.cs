using Application.DTOs.RequestLabTests;
using Application.Services.Abstraction;
using AutoMapper;
using Domain.IRepository;
using Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RequestLabTestService : IRequestLabTestService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public RequestLabTestService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<RequestLabTestReadDto> UpdateLabTestStatusAsync(int requestLabId, int labTestId, RequestLabTestStatus status)
        {
            // Validate RequestLab exists
            var request = await _uow.RequestLabs.GetByIdAsync(requestLabId)
                ?? throw new Application.Common.NotFoundException("RequestLabs", requestLabId);

            // Validate LabTest exists
            var labTest = await _uow.LabTests.GetByIdAsync(labTestId)
                ?? throw new Application.Common.NotFoundException("LabTest", labTestId);

            // Validate relationship exists
            var rel = await _uow.RequestLabTests.GetByIdsAsync(requestLabId, labTestId)
                ?? throw new Application.Common.NotFoundException("RequestLabTest", $"{requestLabId}-{labTestId}");

            // TODO: validate status transitions if needed

            await _uow.RequestLabTests.UpdateStatusAsync(requestLabId, labTestId, status);

            // After updating the individual test status, check if all tests for the request are completed
            var allForRequest = (await _uow.RequestLabTests.GetByRequestLabAsync(requestLabId)).ToList();

            if (allForRequest.Any() && allForRequest.All(x => x.Status == RequestLabTestStatus.Completed))
            {
                // mark parent RequestLabs as Completed if not already
                if (request.Status != Domain.Enums.LabRequestStatus.Completed)
                {
                    request.UpdateStatus(Domain.Enums.LabRequestStatus.Completed);
                    await _uow.RequestLabs.UpdateAsync(request);
                }
            }

            var updated = await _uow.RequestLabTests.GetByIdsAsync(requestLabId, labTestId)
                ?? throw new Application.Common.NotFoundException("RequestLabTest", $"{requestLabId}-{labTestId}");

            return _mapper.Map<RequestLabTestReadDto>(updated);
        }

        public async Task<IEnumerable<RequestLabTestSummaryDto>> GetPendingTestsAsync(int requestLabId)
        {
            var request = await _uow.RequestLabs.GetByIdAsync(requestLabId)
                ?? throw new Application.Common.NotFoundException("RequestLabs", requestLabId);

            var items = await _uow.RequestLabTests.GetPendingTestsAsync(requestLabId);

            return _mapper.Map<IEnumerable<RequestLabTestSummaryDto>>(items);
        }

        public async Task<IEnumerable<RequestLabTestSummaryDto>> GetCompletedTestsAsync(int requestLabId)
        {
            var request = await _uow.RequestLabs.GetByIdAsync(requestLabId)
                ?? throw new Application.Common.NotFoundException("RequestLabs", requestLabId);

            var items = await _uow.RequestLabTests.GetCompletedTestsAsync(requestLabId);

            return _mapper.Map<IEnumerable<RequestLabTestSummaryDto>>(items);
        }

        public async Task<IEnumerable<RequestLabTestReadDto>> GetAllTestsForRequestAsync(int requestLabId)
        {
            var request = await _uow.RequestLabs.GetByIdAsync(requestLabId)
                ?? throw new Application.Common.NotFoundException("RequestLabs", requestLabId);

            var items = await _uow.RequestLabTests.GetByRequestLabAsync(requestLabId);

            return _mapper.Map<IEnumerable<RequestLabTestReadDto>>(items);
        }
    }
}
