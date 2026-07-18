using Application.DTOs.Patient;
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
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        [HasPermission(Permissions.ReadPatient)]
        public async Task<ActionResult<PaginatedResult<PatientReadDto>>> GetAll(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _patientService.GetAllAsync(new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{ssn}")]
        [HasPermission(Permissions.ReadPatient)]
        public async Task<ActionResult<PatientReadDto>> GetBySSN(string ssn)
        {
            var result = await _patientService.GetBySSNAsync(ssn);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CreatePatient)]
        public async Task<ActionResult<PatientReadDto>> Create([FromBody] PatientCreateDto dto)
        {
            var result = await _patientService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetBySSN), new { ssn = dto.NationalId }, result);
        }

        [HttpPut("{ssn}")]
        [HasPermission(Permissions.UpdatePatient)]
        public async Task<ActionResult<PatientReadDto>> Update(string ssn, [FromBody] PatientUpdateDto dto)
        {
            var result = await _patientService.UpdateAsync(ssn, dto);
            return Ok(result);
        }

        [HttpDelete("{ssn}")]
        [HasPermission(Permissions.DeletePatient)]
        public async Task<IActionResult> Delete(string ssn)
        {
            await _patientService.DeleteAsync(ssn);
            return NoContent();
        }
    }
}
