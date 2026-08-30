using MedFlow.Application.Contracts.Conversation;
using MedFlow.Application.Exceptions;
using MedFlow.Application.Security;
using MedFlow.Application.UseCases.Conversation.Interfaces;
using MedFlow.Infrastructure.Persistence.Interfaces;

namespace MedFlow.Application.UseCases.Conversation;

public sealed class ListConversationMessagesUseCase(
    IConversationRepository conversationRepository,
    IResourceAuthorizationService authorizationService) : IListConversationMessagesUseCase
{
    private const int MaxPageSize = 100;

    public async Task<IReadOnlyList<MessageResponse>> ExecuteAsync(
        Guid conversationId,
        Guid? patientId,
        Guid? doctorId,
        int page,
        int pageSize)
    {
        if (!await authorizationService.CanAccessConversationAsync(conversationId, patientId, doctorId))
            throw new ForbiddenException("Você não tem acesso a esta conversa.");

        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        var skip = (normalizedPage - 1) * normalizedPageSize;

        var messages = await conversationRepository.GetMessagesAsync(conversationId, skip, normalizedPageSize);
        return messages
            .Select(message => new MessageResponse(
                message.Id,
                message.ConversationId,
                message.SenderUserId,
                message.Content,
                message.SentAt))
            .ToList();
    }
}
