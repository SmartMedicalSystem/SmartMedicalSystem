using Application.DTOs.Laboratory;
using Application.Services.Abstraction;
using Application.Services.Auth;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class LaboratoriesController : ControllerBase
    {
        private readonly ILaboratoryService _laboratoryService;

        public LaboratoriesController(ILaboratoryService laboratoryService)
        {
            _laboratoryService = laboratoryService;
        }

        // ============================================================
        // 1. GET ALL (PAGINATED)
        // ============================================================
        [HttpGet]
    //    [HasPermission(Permissions.ReadLaboratory)]
        public async Task<ActionResult<PaginatedResult<LaboratoryReadDto>>> GetAll(
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? searchTerm = null,
      [FromQuery] string? statusFilter = null)
        {
            var pagination = new PaginationParams(pageNumber, pageSize);

            // Pass searchTerm and statusFilter directly to service
            var result = await _laboratoryService.GetAllAsync(pagination, searchTerm, statusFilter);
            return Ok(result);
        }

        // ============================================================
        // 2. GET BY ID
        // ============================================================
        [HttpGet("{id:int}")]
      //  [HasPermission(Permissions.ReadLaboratory)]
        public async Task<ActionResult<LaboratoryReadDto>> GetById(int id)
        {
            var result = await _laboratoryService.GetByIdAsync(id);
            return Ok(result);
        }

        // ============================================================
        // 3. GET ACTIVE LABORATORIES
        // ============================================================
        [HttpGet("active")]
   //     [HasPermission(Permissions.ReadLaboratory)]
        public async Task<ActionResult<IEnumerable<LaboratoryReadDto>>> GetActive()
        {
            var result = await _laboratoryService.GetActiveLaboratoriesAsync();
            return Ok(result);
        }

        // ============================================================
        // 4. GET FOR SELECT
        // ============================================================
        [HttpGet("for-select")]
      //  [HasPermission(Permissions.ReadLaboratory)]
        public async Task<ActionResult<IEnumerable<LaboratoryForSelectDto>>> GetForSelect()
        {
            var result = await _laboratoryService.GetForSelectAsync();
            return Ok(result);
        }

        // ============================================================
        // 5. CHECK UNIQUE NAME
        // ============================================================
        [HttpGet("unique")]
     //   [HasPermission(Permissions.ReadLaboratory)]
        public async Task<ActionResult<bool>> IsNameUnique(
            [FromQuery] string name,
            [FromQuery] int? excludeId = null)
        {
            var result = await _laboratoryService.IsLaboratoryNameUniqueAsync(name, excludeId);
            return Ok(result);
        }

        // ============================================================
        // 6. CREATE
        // ============================================================
        [HttpPost]
      //  [HasPermission(Permissions.CreateLaboratory)]
        public async Task<ActionResult<LaboratoryReadDto>> Create([FromBody] LaboratoryCreateDto dto)
        {
            var result = await _laboratoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // ============================================================
        // 7. UPDATE
        // ============================================================
        [HttpPut("{id:int}")]
       // [HasPermission(Permissions.UpdateLaboratory)]
        public async Task<ActionResult<LaboratoryReadDto>> Update(int id, [FromBody] LaboratoryUpdateDto dto)
        {
            var result = await _laboratoryService.UpdateAsync(id, dto);
            return Ok(result);
        }

        // ============================================================
        // GET LAB TESTS BY LABORATORY WITH PAGINATION
        // ============================================================
  

        // ============================================================
        // 8. DELETE
        // ============================================================
        [HttpDelete("{id:int}")]
     //   [HasPermission(Permissions.DeleteLaboratory)]
        public async Task<IActionResult> Delete(int id)
        {
            await _laboratoryService.DeleteAsync(id);
            return NoContent();
        }
    }
}
