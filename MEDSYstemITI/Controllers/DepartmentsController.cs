using Application.DTOs.Department;
using Application.Services.Abstraction;
using Application.Services.Auth;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        // ============================================================
        // 1. GET ALL (PAGINATED) with Search + Status Filter
        // ============================================================
        // في DepartmentsController
        [HttpGet]
       // [HasPermission(Permissions.ReadDepartment)]
        public async Task<ActionResult<PaginatedResult<DepartmentReadDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? statusFilter = null,
            [FromQuery] string? creationDateFilter = null,  // ← NEW
            [FromQuery] string? managerFilter = null)       // ← NEW
        {
            var pagination = new PaginationParams(pageNumber, pageSize);

            var result = await _departmentService.GetAllAsync(
                pagination,
                searchTerm,
                statusFilter,
                creationDateFilter,
                managerFilter
            );

            return Ok(result);
        }

        // ============================================================
        // 2. GET BY ID
        // ============================================================
        [HttpGet("{id:int}")]
      //  [HasPermission(Permissions.ReadDepartment)]
        public async Task<ActionResult<DepartmentReadDto>> GetById(int id)
        {
            var result = await _departmentService.GetByIdAsync(id);
            return Ok(result);
        }

        // ============================================================
        // 3. GET ACTIVE DEPARTMENTS
        // ============================================================
        [HttpGet("active")]
      //  [HasPermission(Permissions.ReadDepartment)]
        public async Task<ActionResult<IEnumerable<DepartmentReadDto>>> GetActive()
        {
            var result = await _departmentService.GetActiveDepartmentsAsync();
            return Ok(result);
        }

        // ============================================================
        // 4. CHECK UNIQUE NAME
        // ============================================================
        [HttpGet("unique")]
      //  [HasPermission(Permissions.ReadDepartment)]
        public async Task<ActionResult<bool>> IsNameUnique(
            [FromQuery] string name,
            [FromQuery] int? excludeId = null)
        {
            var result = await _departmentService.IsDepartmentNameUniqueAsync(name, excludeId);
            return Ok(result);
        }

        // ============================================================
        // 5. CREATE
        // ============================================================
        [HttpPost]
       // [HasPermission(Permissions.CreateDepartment)]
        public async Task<ActionResult<DepartmentReadDto>> Create([FromBody] DepartmentCreateDto dto)
        {
            var result = await _departmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // ============================================================
        // 6. UPDATE
        // ============================================================
        [HttpPut("{id:int}")]
      //  [HasPermission(Permissions.UpdateDepartment)]
        public async Task<ActionResult<DepartmentReadDto>> Update(int id, [FromBody] DepartmentUpdateDto dto)
        {
            var result = await _departmentService.UpdateAsync(id, dto);
            return Ok(result);
        }

        // ============================================================
        // 7. ASSIGN HEAD DOCTOR
        // ============================================================
        [HttpPut("{id:int}/head-doctor/{doctorId:int}")]
       // [HasPermission(Permissions.UpdateDepartment)]
        public async Task<IActionResult> AssignHeadDoctor(int id, int doctorId)
        {
            await _departmentService.AssignHeadDoctorAsync(id, doctorId);
            return NoContent();
        }

        // ============================================================
        // 8. DELETE (SOFT DELETE)
        // ============================================================
        [HttpDelete("{id:int}")]
       // [HasPermission(Permissions.DeleteDepartment)]
        public async Task<IActionResult> Delete(int id)
        {
            await _departmentService.DeleteAsync(id);
            return NoContent();
        }
    }
}