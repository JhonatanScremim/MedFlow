using MedFlow.Application.Contracts.Conversation;
using MedFlow.Application.Exceptions;
using MedFlow.Application.Realtime;
using MedFlow.Application.Security;
using MedFlow.Application.UseCases.Conversation.Interfaces;
using MedFlow.Domain.Entities;
using MedFlow.Infrastructure.Persistence.Interfaces;

namespace MedFlow.Application.UseCases.Conversation;

public sealed class SendMessageUseCase(
    IConversationRepository conversationRepository,
    IResourceAuthorizationService authorizationService,
    INotificationService notificationService) : ISendMessageUseCase
{
    public async Task<MessageResponse> ExecuteAsync(
        Guid conversationId,
        Guid senderUserId,
        Guid? patientId,
        Guid? doctorId,
        SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new BadRequestException("Informe o conteúdo da mensagem.");

        if (!await authorizationService.CanAccessConversationAsync(conversationId, patientId, doctorId))
            throw new ForbiddenException("Você não tem acesso a esta conversa.");

        var message = await conversationRepository.CreateMessageAsync(new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderUserId = senderUserId,
            Content = request.Content.Trim(),
            SentAt = DateTimeOffset.UtcNow
        });

        var response = new MessageResponse(
            message.Id,
            message.ConversationId,
            message.SenderUserId,
            message.Content,
            message.SentAt);

        await notificationService.NotifyMessageSentAsync(response);

        return response;
    }
}
