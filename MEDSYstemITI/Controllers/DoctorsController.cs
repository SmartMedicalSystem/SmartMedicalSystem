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

        [HttpGet]
        [HasPermission(Permissions.ReadDoctor)]
        public async Task<ActionResult<PaginatedResult<DoctorReadDto>>> GetAll(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _doctorService.GetAllAsync(new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{ssn}")]
        [HasPermission(Permissions.ReadDoctor)]
        public async Task<ActionResult<DoctorReadDto>> GetBySSN(string ssn)
        {
            var result = await _doctorService.GetBySSNAsync(ssn);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CreateDoctor)]
        public async Task<ActionResult<DoctorReadDto>> Create([FromBody] DoctorCreateDto dto)
        {
            var result = await _doctorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetBySSN), new { ssn = dto.NationalId }, result);
        }

        [HttpPut("{ssn}")]
        [HasPermission(Permissions.UpdateDoctor)]
        public async Task<ActionResult<DoctorReadDto>> Update(string ssn, [FromBody] DoctorUpdateDto dto)
        {
            var result = await _doctorService.UpdateAsync(ssn, dto);
            return Ok(result);
        }

        [HttpDelete("{ssn}")]
        [HasPermission(Permissions.DeleteDoctor)]
        public async Task<IActionResult> Delete(string ssn)
        {
            await _doctorService.DeleteAsync(ssn);
            return NoContent();
        }
    }
}
