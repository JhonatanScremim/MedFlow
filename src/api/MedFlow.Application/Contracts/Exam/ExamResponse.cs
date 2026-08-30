using MedFlow.Domain.Enums;

namespace MedFlow.Application.Contracts.Exam;

public sealed record ExamResponse(
    Guid Id,
    Guid PatientId,
    Guid? DoctorId,
    Guid? ConversationId,
    ExamType Type,
    ExamStatus Status,
    DateTimeOffset? ScheduledAtUtc,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
