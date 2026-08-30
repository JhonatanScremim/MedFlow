using MedFlow.Application.Contracts.Conversation;

namespace MedFlow.Application.UseCases.Conversation.Interfaces;

public interface IListConversationsUseCase
{
    Task<IReadOnlyList<ConversationResponse>> ExecuteAsync(Guid? patientId, Guid? doctorId);
}
