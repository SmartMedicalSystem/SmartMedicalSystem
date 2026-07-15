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

        [HttpGet("{ssn}")]
        [HasPermission(Permissions.ReadLabTechnician)]
        public async Task<ActionResult<LabTechnicianReadDto>> GetBySSN(string ssn)
        {
            var result = await _labTechnicianService.GetBySSNAsync(ssn);
            return Ok(result);
        }

        [HttpPost]
        //[HasPermission(Permissions.CreateLabTechnician)]
        public async Task<ActionResult<LabTechnicianReadDto>> Create(
            [FromForm] LabTechnicianCreateDto dto)
        {
            var result = await _labTechnicianService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetBySSN), new { ssn = dto.NationalId }, result);
        }

        [HttpPut("{ssn}")]
        [HasPermission(Permissions.UpdateLabTechnician)]
        public async Task<ActionResult<LabTechnicianReadDto>> Update(string ssn, [FromBody] LabTechnicianUpdateDto dto)
        {
            var result = await _labTechnicianService.UpdateAsync(ssn, dto);
            return Ok(result);
        }

        [HttpDelete("{ssn}")]
        [HasPermission(Permissions.DeleteLabTechnician)]
        public async Task<IActionResult> Delete(string ssn)
        {
            await _labTechnicianService.DeleteAsync(ssn);
            return NoContent();
        }
    }
}