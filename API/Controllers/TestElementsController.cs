namespace API.Controllers
{
    public class TestElementsController: ControllerBase
    {
        private readonly ITestElementService _testElementService;

        public TestElementsController(ITestElementService testElementService)
        {
            _testElementService = testElementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _testElementService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _testElementService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Test element with ID {id} is not found" });

            return Ok(result);
        }

        [HttpGet("by-test/{testId}")]
        public async Task<IActionResult> GetByTestId(int testId)
        {
            var result = await _testElementService.GetByTestIdAsync(testId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTestElementDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _testElementService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.TestElementId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTestElementDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _testElementService.UpdateAsync(id, dto);
            if (!success)
                return NotFound(new { message = $"Test element with ID {id} is not found" });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _testElementService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = $"Test element with ID {id} is not found" });

            return NoContent();
        }
    }
}

