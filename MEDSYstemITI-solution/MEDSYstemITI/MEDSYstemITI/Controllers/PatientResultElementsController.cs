using Application.DTOs.PatientResultElement;
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
    public class PatientResultElementsController : ControllerBase
    {
        private readonly IPatientResultElementService _patientResultElementService;

        public PatientResultElementsController(IPatientResultElementService patientResultElementService)
        {
            _patientResultElementService = patientResultElementService;
        }

        [HttpGet("by-patient-result/{patientResultId:int}")]
        [HasPermission(Permissions.ReadLabReport)]
        public async Task<ActionResult<PaginatedResult<PatientResultElementReadDto>>> GetByPatientResult(
            int patientResultId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _patientResultElementService.GetByPatientResultAsync(patientResultId, new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        /// <summary>Lab technician records a test element's measured value for a patient result.</summary>
        [HttpPost]
        [HasPermission(Permissions.CreateLabReport)]
        public async Task<ActionResult<PatientResultElementReadDto>> Create([FromBody] PatientResultElementCreateDto dto)
        {
            var result = await _patientResultElementService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByPatientResult), new { patientResultId = result.PatientResultId }, result);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.UpdateLabReport)]
        public async Task<ActionResult<PatientResultElementReadDto>> Update(int id, [FromBody] PatientResultElementUpdateDto dto)
        {
            var result = await _patientResultElementService.UpdateAsync(id, dto);
            return Ok(result);
        }
    }
}
