using Application.DTOs.AIChat;

namespace Application.Services.Abstraction.AI;

public interface IFunctionCallingChatService
{
    Task<AIChatResponseDto> ChatAsync(
        AIChatRequestDto request,
        CancellationToken cancellationToken = default);
}
