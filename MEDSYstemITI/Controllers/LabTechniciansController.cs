using Application.DTOs.Auth;
using Application.DTOs.LabTechnician;
using Application.Services.Abstraction;
using Application.Services.Abstraction.Auth;
using Application.Services.Auth;
using Domain.Enums;
using Domain.Models;
using MEDSYstemITI.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

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
        //[HasPermission(Permissions.ReadLabTechnician)]
        public async Task<ActionResult<LabTechnicianReadDto>> GetBySSN(string ssn)
        {
            var result = await _labTechnicianService.GetBySSNAsync(ssn);
            return Ok(result);
        }

        [HttpPost("create")]
        //[HasPermission(Permissions.CreateLabTechnician)]
        public async Task<ActionResult<LabTechnicianReadDto>> Create([FromForm]   LabTechnicianCreateDto dto)
        {
            var person = await _labTechnicianService.CreateAsync(dto);
            

            return Ok(person);



        }

        [HttpPut("{ssn}")]
        //[HasPermission(Permissions.UpdateLabTechnician)]
        public async Task<ActionResult<LabTechnicianReadDto>> Update(string ssn, [FromForm] LabTechnicianUpdateDto dto)
        {
            var result = await _labTechnicianService.UpdateAsync(ssn, dto);
            return Ok(result);
        }

        [HttpDelete("{ssn}")]
        [HasPermission(Permissions.DeleteLabTechnician)]
        [LogSensitiveAction]

        public async Task<IActionResult> Delete(string ssn)
        {
            await _labTechnicianService.DeleteAsync(ssn);
            return NoContent();
        }

        // ===== NEW: Get technicians by Laboratory with Pagination =====
        [HttpGet("by-laboratory/{laboratoryId:int}")]
        [HasPermission(Permissions.ReadLabTechnician)]
        public async Task<ActionResult<PaginatedResult<LabTechnicianReadDto>>> GetByLaboratory(
            int laboratoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? searchTerm = null)
        {
            var pagination = new PaginationParams(pageNumber, pageSize);
            var result = await _labTechnicianService.GetByLaboratoryIdAsync(laboratoryId, pagination, searchTerm);
            return Ok(result);
        }

        [HttpGet("available-for-laboratory/{laboratoryId:int}")]
        [HasPermission(Permissions.ReadLabTechnician)]
        public async Task<
    ActionResult<PaginatedResult<LabTechnicianReadDto>>>
    GetAvailableForLaboratory(
        int laboratoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
        {
            var pagination =
                new PaginationParams(pageNumber, pageSize);

            var result =
                await _labTechnicianService
                    .GetAvailableForLaboratoryAsync(
                        laboratoryId,
                        pagination,
                        searchTerm);

            return Ok(result);
        }
    }
}