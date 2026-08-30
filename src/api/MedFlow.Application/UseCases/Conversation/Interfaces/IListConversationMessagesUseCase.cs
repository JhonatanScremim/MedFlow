using MedFlow.Application.Contracts.Conversation;

namespace MedFlow.Application.UseCases.Conversation.Interfaces;

public interface IListConversationMessagesUseCase
{
    Task<IReadOnlyList<MessageResponse>> ExecuteAsync(
        Guid conversationId,
        Guid? patientId,
        Guid? doctorId,
        int page,
        int pageSize);
}
