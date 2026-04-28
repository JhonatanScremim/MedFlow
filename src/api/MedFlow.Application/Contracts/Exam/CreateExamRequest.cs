using System.ComponentModel.DataAnnotations;
using MedFlow.Domain.Enums;

namespace MedFlow.Application.Contracts.Exam;

public sealed class CreateExamRequest
{
    [Required]
    public ExamType Type { get; set; }

    public Guid? DoctorId { get; set; }

    public DateTimeOffset? ScheduledAtUtc { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}
