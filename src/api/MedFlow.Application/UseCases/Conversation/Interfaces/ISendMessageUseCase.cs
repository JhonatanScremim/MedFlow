using MedFlow.Application.Contracts.Conversation;

namespace MedFlow.Application.UseCases.Conversation.Interfaces;

public interface ISendMessageUseCase
{
    Task<MessageResponse> ExecuteAsync(
        Guid conversationId,
        Guid senderUserId,
        Guid? patientId,
        Guid? doctorId,
        SendMessageRequest request);
}
