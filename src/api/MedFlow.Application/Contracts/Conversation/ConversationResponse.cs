namespace MedFlow.Application.Contracts.Conversation;

public sealed record ConversationResponse(
    Guid Id,
    Guid ExamId,
    Guid PatientId,
    Guid? DoctorId,
    DateTimeOffset CreatedAt);
