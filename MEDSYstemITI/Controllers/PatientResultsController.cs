using Application.DTOs.PatientResult;
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
    public class PatientResultsController : ControllerBase
    {
        private readonly IPatientResultService _patientResultService;

        public PatientResultsController(IPatientResultService patientResultService)
        {
            _patientResultService = patientResultService;
        }

        [HttpGet("by-patient/{patientId:int}")]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<ActionResult<PaginatedResult<PatientResultReadDto>>> GetByPatient(
            int patientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _patientResultService.GetByPatientAsync(patientId, new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<ActionResult<PatientResultReadDto>> GetById(int id)
        {
            var result = await _patientResultService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CreateLabReport)]
        public async Task<ActionResult<PatientResultReadDto>> Create([FromBody] PatientResultCreateDto dto)
        {
            var result = await _patientResultService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.UpdateLabReport)]
        public async Task<ActionResult<PatientResultReadDto>> Update(int id, [FromBody] PatientResultUpdateDto dto)
        {
            var result = await _patientResultService.UpdateAsync(id, dto);
            return Ok(result);
        }
    }
}
