using Application.DTOs.Doctor;
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
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet("by-department/{departmentId:int}")]
        [HasPermission(Permissions.ReadDoctor)]
        public async Task<ActionResult<PaginatedResult<DoctorReadDto>>> GetByDepartment(
            int departmentId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _doctorService.GetByDepartmentAsync(departmentId, new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ReadDoctor)]
        public async Task<ActionResult<DoctorReadDto>> GetById(int id)
        {
            var result = await _doctorService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CreateDoctor)]
        public async Task<ActionResult<DoctorReadDto>> Create([FromBody] DoctorCreateDto dto)
        {
            var result = await _doctorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.UpdateDoctor)]
        public async Task<ActionResult<DoctorReadDto>> Update(int id, [FromBody] DoctorUpdateDto dto)
        {
            var result = await _doctorService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.DeleteDoctor)]
        public async Task<IActionResult> Delete(int id)
        {
            await _doctorService.DeleteAsync(id);
            return NoContent();
        }
    }
}
