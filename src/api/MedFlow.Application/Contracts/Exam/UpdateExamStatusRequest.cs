using System.ComponentModel.DataAnnotations;
using MedFlow.Domain.Enums;

namespace MedFlow.Application.Contracts.Exam;

public sealed class UpdateExamStatusRequest
{
    [Required]
    public ExamStatus Status { get; set; }
}
