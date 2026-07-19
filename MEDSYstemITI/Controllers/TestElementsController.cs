using Application.DTOs.TestElement;
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
    public class TestElementsController : ControllerBase
    {
        private readonly ITestElementService _testElementService;

        public TestElementsController(ITestElementService testElementService)
        {
            _testElementService = testElementService;
        }

        [HttpGet]
        [HasPermission(Permissions.ReadTestElement)]
        public async Task<ActionResult<PaginatedResult<TestElementReadDto>>> GetAll(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _testElementService.GetAllAsync(new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ReadTestElement)]
        public async Task<ActionResult<TestElementReadDto>> GetById(int id)
        {
            var result = await _testElementService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CreateTestElement)]
        public async Task<ActionResult<TestElementReadDto>> Create([FromBody] TestElementCreateDto dto)
        {
            var result = await _testElementService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.UpdateTestElement)]
        public async Task<ActionResult<TestElementReadDto>> Update(int id, [FromBody] TestElementUpdateDto dto)
        {
            var result = await _testElementService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.DeleteTestElement)]
        public async Task<IActionResult> Delete(int id)
        {
            await _testElementService.DeleteAsync(id);
            return NoContent();
        }
    }
}
