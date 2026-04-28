using MedFlow.Domain.Enums;

namespace MedFlow.Application.Contracts.Exam;

public sealed record CreateExamResponse(
    Guid Id,
    Guid PatientId,
    Guid? DoctorId,
    ExamType Type,
    ExamStatus Status,
    DateTimeOffset? ScheduledAtUtc,
    string? Notes,
    DateTimeOffset CreatedAt);
