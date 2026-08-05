using Application.DTOs.RequestLabTests;
using Application.Services.Abstraction;
using Domain.Enums;
using MEDSYstemITI;
using Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers
{
    [ApiController]
    [Route("api/request-labs/{requestLabId:int}/lab-tests")]
    [Authorize]
    public class RequestLabTestsController : ControllerBase
    {
        private readonly IRequestLabTestService _service;

        public RequestLabTestsController(IRequestLabTestService service)
        {
            _service = service;
        }

        [HttpPatch("{labTestId:int}/status")]
        [HasPermission(Permissions.UpdateLabReport)]
        public async Task<IActionResult> UpdateStatus(int requestLabId, int labTestId, [FromBody] RequestLabTestStatusUpdateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var updated = await _service.UpdateLabTestStatusAsync(requestLabId, labTestId, dto.Status);
            return Ok(updated);
        }

        [HttpGet("pending")]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<IActionResult> GetPending(int requestLabId)
        {
            var items = await _service.GetPendingTestsAsync(requestLabId);
            return Ok(items);
        }

        [HttpGet("completed")]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<IActionResult> GetCompleted(int requestLabId)
        {
            var items = await _service.GetCompletedTestsAsync(requestLabId);
            return Ok(items);
        }

        [HttpGet]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<IActionResult> GetAll(int requestLabId)
        {
            var items = await _service.GetAllTestsForRequestAsync(requestLabId);
            return Ok(items);
        }
    }
}
