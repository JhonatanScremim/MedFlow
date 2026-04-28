using MedFlow.Application.Contracts.Exam;

namespace MedFlow.Application.UseCases.Exam.Interfaces;

public interface ICreateExamUseCase
{
    Task<CreateExamResponse> ExecuteAsync(Guid patientId, CreateExamRequest request);
}
