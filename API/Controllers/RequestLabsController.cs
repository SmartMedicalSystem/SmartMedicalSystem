using Application.DTOs.RequestLabsDtos;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestLabsController : ControllerBase
    {
        private readonly IRequestLabsService _service;

        public RequestLabsController(
            IRequestLabsService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateRequestLabsDto dto)
        {
            var result =
                await _service.CreateAsync(dto);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _service.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var result =
                await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult>
            GetBySession(int sessionId)
        {
            var result =
                await _service
                    .GetBySessionIdAsync(sessionId);

            return Ok(result);
        }

        [HttpGet("pending")]
        public async Task<IActionResult>
            GetPending()
        {
            var result =
                await _service.GetPendingAsync();

            return Ok(result);
        }

        [HttpPut("{id}/start")]
        public async Task<IActionResult>
            Start(int id)
        {
            await _service.StartProcessingAsync(id);

            return NoContent();
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult>
            Complete(int id)
        {
            await _service.CompleteAsync(id);

            return NoContent();
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult>
            Cancel(int id)
        {
            await _service.CancelAsync(id);

            return NoContent();
        }
    }
}
