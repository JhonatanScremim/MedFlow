using MedFlow.Application.Contracts.Exam;

namespace MedFlow.Application.UseCases.Exam.Interfaces;

public interface IListExamsUseCase
{
    Task<IReadOnlyList<ExamResponse>> ExecuteAsync(Guid? patientId, Guid? doctorId);
}
