using Application.DTOs.AIChat;
using Application.Services.Abstraction.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MEDSYstemITI.Controllers;

[ApiController]
[Route("api/AIChat")]
[Authorize]
public sealed class AIChatController : ControllerBase
{
    private readonly IFunctionCallingChatService _chatService;

    public AIChatController(IFunctionCallingChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<ActionResult<AIChatResponseDto>> Chat(
        [FromBody] AIChatRequestDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message) && string.IsNullOrWhiteSpace(request.ConfirmActionId))
            return BadRequest(new { message = "Message is required." });

        var response = await _chatService.ChatAsync(request, cancellationToken);
        return Ok(response);
    }
}
