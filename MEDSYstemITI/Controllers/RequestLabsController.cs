using Application.DTOs.RequestLabs;
using Application.Services.Abstraction;
using Application.Services.Auth;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RequestLabsController : ControllerBase
    {
        private readonly IRequestLabsService _requestLabsService;

        public RequestLabsController(IRequestLabsService requestLabsService)
        {
            _requestLabsService = requestLabsService;
        }

        [HttpGet("by-session/{sessionId:int}")]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<ActionResult<PaginatedResult<RequestLabsReadDto>>> GetBySession(
            int sessionId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _requestLabsService.GetBySessionAsync(sessionId, new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<ActionResult<RequestLabsReadDto>> GetById(int id)
        {
            var result = await _requestLabsService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>A doctor requests one or more lab tests for a session.</summary>
        [HttpPost]
        [HasPermission(Permissions.RequestLabTest)]
        public async Task<ActionResult<RequestLabsReadDto>> Create([FromBody] RequestLabsCreateDto dto)
        {
            var result = await _requestLabsService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Lab technician/admin updates the status (Pending -> InProgress -> Completed, etc).</summary>
        [HttpPut("{id:int}/status")]
        [HasPermission(Permissions.UpdateLabReport)]
        public async Task<ActionResult<RequestLabsReadDto>> UpdateStatus(int id, [FromBody] RequestLabsUpdateStatusDto dto)
        {
            var result = await _requestLabsService.UpdateStatusAsync(id, dto);
            return Ok(result);
        }


        [HttpGet("pending")]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<ActionResult<PaginatedResult<RequestLabsReadDto>>> GetpendingRequests([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
           
            PaginationParams paginationParams = new PaginationParams(pageNumber, pageSize); // Default pagination parameters
            var result = await _requestLabsService.GetPendingRequestsAsync(paginationParams);
            return Ok(result);
        }

        [HttpGet("completed")]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<ActionResult<PaginatedResult<RequestLabsReadDto>>> GetCompletedRequests([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            PaginationParams paginationParams = new PaginationParams(pageNumber, pageSize); // Default pagination parameters
            var result = await _requestLabsService.GetByStatusAsync(LabRequestStatus.Completed, paginationParams);
            return Ok(result);
        }



        [HttpGet("laboratory")]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<ActionResult<PaginatedResult<RequestLabsReadDto>>> GetLaboratory(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] LabRequestStatus? status = null,
            [FromQuery] LabRequestPriority? priority = null,
            [FromQuery] int? labTestId = null,
            [FromQuery] int? doctorId = null)
        {
            var result = await _requestLabsService.QueryAsync(
                new PaginationParams(pageNumber, pageSize),
                search, status, priority, labTestId, doctorId);

            return Ok(result);
        }

        [HttpGet("laboratory/statistics")]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<ActionResult<RequestLabsStatisticsDto>> GetLaboratoryStatistics()
        {
            var result = await _requestLabsService.GetStatisticsAsync();
            return Ok(result);
        }
    }
}
