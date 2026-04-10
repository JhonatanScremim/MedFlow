using MedFlow.Domain.Enums;

namespace MedFlow.Domain.Entities;

public class Exam
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public ExamType Type { get; set; }
    public ExamStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Patient Patient { get; set; } = null!;
    public Conversation? Conversation { get; set; }
}
