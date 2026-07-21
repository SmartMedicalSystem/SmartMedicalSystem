using Application.DTOs.Element;
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
    public class ElementsController : ControllerBase
    {
        private readonly IElementService _elementService;

        public ElementsController(IElementService elementService)
        {
            _elementService = elementService;
        }

        [HttpGet]
        [HasPermission(Permissions.ReadTestElement)]
        public async Task<ActionResult<PaginatedResult<ElementReadDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _elementService.GetAllAsync(new PaginationParams(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.ReadTestElement)]
        public async Task<ActionResult<ElementReadDto>> GetById(int id)
        {
            var result = await _elementService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CreateTestElement)]
        public async Task<ActionResult<ElementReadDto>> Create([FromBody] ElementCreateDto dto)
        {
            var result = await _elementService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.UpdateTestElement)]
        public async Task<ActionResult<ElementReadDto>> Update(int id, [FromBody] ElementUpdateDto dto)
        {
            var result = await _elementService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.DeleteTestElement)]
        public async Task<IActionResult> Delete(int id)
        {
            await _elementService.DeleteAsync(id);
            return NoContent();
        }
    }
}
