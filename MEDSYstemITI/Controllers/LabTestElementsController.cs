using Application.DTOs.LabTestElement;
using Application.Services.Abstraction;
using Application.Services.Auth;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LabTestElementsController : ControllerBase
    {
        private readonly ILabTestElementService _labTestElementService;

        public LabTestElementsController(ILabTestElementService labTestElementService)
        {
            _labTestElementService = labTestElementService;
        }

        [HttpGet("by-lab-test/{labTestId:int}")]
        [HasPermission(Permissions.ReadLabTest)]
        public async Task<ActionResult<IEnumerable<LabTestElementReadDto>>> GetByLabTest(int labTestId)
        {
            var result = await _labTestElementService.GetByLabTestAsync(labTestId);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.UpdateLabTest)]
        public async Task<ActionResult<LabTestElementReadDto>> Add([FromBody] LabTestElementCreateDto dto)
        {
            var result = await _labTestElementService.AddElementToTestAsync(dto);
            return CreatedAtAction(nameof(GetByLabTest), new { labTestId = result.LabTestId }, result);
        }

        [HttpDelete("{labTestId:int}/{testElementId:int}")]
        [HasPermission(Permissions.UpdateLabTest)]
        public async Task<IActionResult> Remove(int labTestId, int testElementId)
        {
            await _labTestElementService.RemoveElementFromTestAsync(labTestId, testElementId);
            return NoContent();
        }
    }
}
