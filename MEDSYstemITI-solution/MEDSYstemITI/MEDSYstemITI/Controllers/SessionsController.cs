using Application.DTOs.Session;
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
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet("by-patient/{patientId:int}")]
        [HasPermission(Permissions.ReadSession)]
        public async Task<ActionResult<PaginatedResult<SessionReadDto>>> GetByPatient(
            int patientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _sessionService.GetByPatientAsync(patientId, new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ReadSession)]
        public async Task<ActionResult<SessionReadDto>> GetById(int id)
        {
            var result = await _sessionService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CreateSession)]
        public async Task<ActionResult<SessionReadDto>> Create([FromBody] SessionCreateDto dto)
        {
            var result = await _sessionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.UpdateSession)]
        public async Task<ActionResult<SessionReadDto>> Update(int id, [FromBody] SessionUpdateDto dto)
        {
            var result = await _sessionService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.DeleteSession)]
        public async Task<IActionResult> Delete(int id)
        {
            await _sessionService.DeleteAsync(id);
            return NoContent();
        }
    }
}
