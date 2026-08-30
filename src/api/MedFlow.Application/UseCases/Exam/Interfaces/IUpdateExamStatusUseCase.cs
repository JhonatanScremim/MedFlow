using MedFlow.Application.Contracts.Exam;

namespace MedFlow.Application.UseCases.Exam.Interfaces;

public interface IUpdateExamStatusUseCase
{
    Task<ExamResponse> ExecuteAsync(Guid doctorId, Guid examId, UpdateExamStatusRequest request);
}
