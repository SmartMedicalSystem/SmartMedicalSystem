using Application.DTOs.Department;
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
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [HasPermission(Permissions.ReadDepartment)]
        public async Task<ActionResult<PaginatedResult<DepartmentReadDto>>> GetAll(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _departmentService.GetAllAsync(new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ReadDepartment)]
        public async Task<ActionResult<DepartmentReadDto>> GetById(int id)
        {
            var result = await _departmentService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CreateDepartment)]
        public async Task<ActionResult<DepartmentReadDto>> Create([FromBody] DepartmentCreateDto dto)
        {
            var result = await _departmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.UpdateDepartment)]
        public async Task<ActionResult<DepartmentReadDto>> Update(int id, [FromBody] DepartmentUpdateDto dto)
        {
            var result = await _departmentService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpPut("{id:int}/head-doctor/{doctorId:int}")]
        [HasPermission(Permissions.UpdateDepartment)]
        public async Task<IActionResult> AssignHeadDoctor(int id, int doctorId)
        {
            await _departmentService.AssignHeadDoctorAsync(id, doctorId);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.DeleteDepartment)]
        public async Task<IActionResult> Delete(int id)
        {
            await _departmentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
