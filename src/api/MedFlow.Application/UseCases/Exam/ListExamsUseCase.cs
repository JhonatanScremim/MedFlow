using AutoMapper;
using MedFlow.Application.Contracts.Exam;
using MedFlow.Application.Exceptions;
using MedFlow.Application.UseCases.Exam.Interfaces;
using MedFlow.Infrastructure.Persistence.Interfaces;

namespace MedFlow.Application.UseCases.Exam;

public sealed class ListExamsUseCase(
    IExamRepository examRepository,
    IMapper mapper) : IListExamsUseCase
{
    public async Task<IReadOnlyList<ExamResponse>> ExecuteAsync(Guid? patientId, Guid? doctorId)
    {
        if (patientId is null && doctorId is null)
            throw new AuthenticationException("Token não possui perfil de paciente ou médico.");

        var exams = doctorId is not null
            ? await examRepository.GetForDoctorDashboardAsync(doctorId.Value)
            : await examRepository.GetForPatientAsync(patientId!.Value);

        return exams.Select(mapper.Map<ExamResponse>).ToList();
    }
}
