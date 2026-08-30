using MedFlow.Application.Contracts.Conversation;
using MedFlow.Application.UseCases.Conversation.Interfaces;
using MedFlow.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedFlow.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public sealed class ConversationsController(
    IListConversationsUseCase listConversationsUseCase,
    IListConversationMessagesUseCase listConversationMessagesUseCase,
    ISendMessageUseCase sendMessageUseCase) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ConversationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<IActionResult> List() =>
        ExecuteSafeAsync(async () =>
        {
            var conversations = await listConversationsUseCase.ExecuteAsync(User.GetPatientId(), User.GetDoctorId());
            return Ok(conversations);
        });

    [HttpGet("{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(IReadOnlyList<MessageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> ListMessages(Guid conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) =>
        ExecuteSafeAsync(async () =>
        {
            var messages = await listConversationMessagesUseCase.ExecuteAsync(
                conversationId,
                User.GetPatientId(),
                User.GetDoctorId(),
                page,
                pageSize);

            return Ok(messages);
        });

    [HttpPost("{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendMessageRequest request) =>
        ExecuteSafeAsync(async () =>
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized(new { message = "Token não possui claim sub." });

            var message = await sendMessageUseCase.ExecuteAsync(
                conversationId,
                userId.Value,
                User.GetPatientId(),
                User.GetDoctorId(),
                request);

            return StatusCode(StatusCodes.Status201Created, message);
        });
}
