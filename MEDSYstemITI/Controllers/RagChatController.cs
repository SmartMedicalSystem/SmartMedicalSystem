using Application.DTOs.Rag;
using Application.Services.Abstraction.AI;
using Application.Services.Auth;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers
{
    /// <summary>
    /// Retrieval-Augmented-Generation endpoints backing the front-end chatbot widget. The vector
    /// store is populated automatically whenever AI reports are generated (see
    /// PatientAIReportsController / PatientAIMcpTools), and can additionally be fed manual notes
    /// through the index endpoint below.
    /// </summary>
    [ApiController]
    [Route("api/rag")]
    //[Authorize]
    public class RagChatController : ControllerBase
    {
        private readonly IRagChatService _ragChatService;
        private readonly IRagService _ragService;

        public RagChatController(IRagChatService ragChatService, IRagService ragService)
        {
            _ragChatService = ragChatService;
            _ragService = ragService;
        }

        /// <summary>
        /// Answers a question using RAG over the patient's (or, if PatientId is omitted, every
        /// patient's) indexed AI summaries/reports/suggestions. This is the endpoint the front-end
        /// chatbot calls.
        /// </summary>
        [HttpPost("chat")]
        //[HasPermission(Permissions.ReadAiReport)]
        public async Task<ActionResult<RagChatResponseDto>> Chat([FromBody] RagChatRequestDto request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Question is required.");

            var response = await _ragChatService.AskAsync(request, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Manually embeds and indexes arbitrary clinical text (e.g. a doctor's free-text note)
        /// into a patient's RAG vector store so future chatbot answers can draw on it.
        /// </summary>
        [HttpPost("index")]
        //[HasPermission(Permissions.UpdateAiReport)]
        public async Task<IActionResult> Index([FromBody] RagIndexRequestDto request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Content is required.");

            await _ragService.IndexAsync(request.PatientId, null, Domain.Enums.RagSourceType.Manual, request.Content, cancellationToken);
            return NoContent();
        }
    }
}
