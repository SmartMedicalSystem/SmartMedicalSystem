using Application.DTOs.LabTechnician;
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
    //[Authorize]
    public class LabTechniciansController : ControllerBase
    {
        private readonly ILabTechnicianService _labTechnicianService;

        public LabTechniciansController(ILabTechnicianService labTechnicianService)
        {
            _labTechnicianService = labTechnicianService;
        }

        [HttpGet]
        //[HasPermission(Permissions.ReadLabTechnician)]
        public async Task<ActionResult<PaginatedResult<LabTechnicianReadDto>>> GetAll(
        [FromQuery] LabTechnicianFilterDto filter)
        {
            var result = await _labTechnicianService.GetAllAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ReadLabTechnician)]
        public async Task<ActionResult<LabTechnicianReadDto>> GetById(int id)
        {
            var result = await _labTechnicianService.GetByIdAsync(id);

            return Ok(result);
        }

        [HttpPost]
        //[HasPermission(Permissions.CreateLabTechnician)]
        public async Task<ActionResult<LabTechnicianReadDto>> Create(
            [FromBody] LabTechnicianCreateDto dto)
        {
            var result = await _labTechnicianService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        //[HasPermission(Permissions.UpdateLabTechnician)]
        public async Task<ActionResult<LabTechnicianReadDto>> Update(
            int id,
            [FromBody] LabTechnicianUpdateDto dto)
        {
            var result = await _labTechnicianService.UpdateAsync(id, dto);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.DeleteLabTechnician)]
        public async Task<IActionResult> Delete(int id)
        {
            await _labTechnicianService.DeleteAsync(id);

            return NoContent();
        }
    }
}