namespace API.Controllers
{
    [ApiController]
    [Route("api/labtests")]
    public class LabTestsController : ControllerBase
    {
        private readonly ILabTestService _service;

        public LabTestsController(ILabTestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateLabTestDto dto)
        {
            await _service.CreateAsync(dto);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateLabTestDto dto)
        {
            var updated =
                await _service.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted =
                await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
