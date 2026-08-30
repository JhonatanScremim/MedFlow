namespace MedFlow.Application.Contracts.Conversation;

public sealed record MessageResponse(
    Guid Id,
    Guid ConversationId,
    Guid? SenderUserId,
    string Content,
    DateTimeOffset SentAt);
