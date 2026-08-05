using Application.DTOs.LabTest;
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
    public class LabTestsController : ControllerBase
    {
        private readonly ILabTestService _labTestService;

        public LabTestsController(ILabTestService labTestService)
        {
            _labTestService = labTestService;
        }

        [HttpGet]
        [HasPermission(Permissions.ReadLabTest)]
        public async Task<ActionResult<PaginatedResult<LabTestReadDto>>> GetAll(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _labTestService.GetAllAsync(new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ReadLabTest)]
        public async Task<ActionResult<LabTestReadDto>> GetById(int id)
        {
            var result = await _labTestService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CreateLabTest)]
        public async Task<ActionResult<LabTestReadDto>> Create([FromBody] LabTestCreateDto dto)
        {
            var result = await _labTestService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.UpdateLabTest)]
        public async Task<ActionResult<LabTestReadDto>> Update(int id, [FromBody] LabTestUpdateDto dto)
        {
            var result = await _labTestService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.DeleteLabTest)]
        public async Task<IActionResult> Delete(int id)
        {
            await _labTestService.DeleteAsync(id);
            return NoContent();
        }

        // ===== NEW: Get Lab Tests by Laboratory with Pagination =====
        [HttpGet("by-laboratory/{laboratoryId:int}")]
        [HasPermission(Permissions.ReadLabTest)]
        public async Task<ActionResult<PaginatedResult<LabTestReadDto>>> GetByLaboratory(
            int laboratoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? searchTerm = null)
        {
            var pagination = new PaginationParams(pageNumber, pageSize);
            var result = await _labTestService.GetByLaboratoryIdAsync(laboratoryId, pagination, searchTerm);
            return Ok(result);
        }



        [HttpGet("Pending")]
        [HasPermission(Permissions.ReadLabTest)]
        public async Task<ActionResult<PaginatedResult<LabTestReadDto>>> GetPendingLabTests(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagination = new PaginationParams(pageNumber, pageSize);
            var result = await _labTestService.GetByStatusAsync(LabTestStatus.Pending, pagination);
            return Ok(result);
        }

        [HttpGet("Completed")]
        [HasPermission(Permissions.ReadLabTest)]
        public async Task<ActionResult<PaginatedResult<LabTestReadDto>>> GetCompletedLabTests(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagination = new PaginationParams(pageNumber, pageSize);
            var result = await _labTestService.GetByStatusAsync(LabTestStatus.Completed, pagination);
            return Ok(result);
        }

        [HttpPut("{id:int}/status")]
        [HasPermission(Permissions.UpdateLabTest)]
        public async Task<ActionResult<LabTestReadDto>> UpdateStatus(int id, [FromBody] LabTestUpdateStatusDto dto)
        {
            var result = await _labTestService.UpdateStatusAsync(id, dto);
            return Ok(result);
        }
    }
}
