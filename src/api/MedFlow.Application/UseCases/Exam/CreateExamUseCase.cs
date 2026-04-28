using MedFlow.Application.Contracts.Exam;
using MedFlow.Application.Exceptions;
using MedFlow.Application.UseCases.Exam.Interfaces;
using AutoMapper;
using MedFlow.Domain.Enums;
using MedFlow.Infrastructure.Persistence.Interfaces;
using ExamEntity = MedFlow.Domain.Entities.Exam;

namespace MedFlow.Application.UseCases.Exam;

public sealed class CreateExamUseCase(
    IBaseRepository<ExamEntity> examRepository,
    IMapper mapper) : ICreateExamUseCase
{
    public async Task<CreateExamResponse> ExecuteAsync(Guid patientId, CreateExamRequest request)
    {
        if (!Enum.IsDefined(typeof(ExamType), request.Type))
            throw new BadRequestException("Tipo de exame inválido.");

        var exam = mapper.Map<ExamEntity>(request, options =>
        {
            options.Items["PatientId"] = patientId;
            options.Items["Now"] = DateTimeOffset.UtcNow;
        });

        await examRepository.CreateAsync(exam);

        return mapper.Map<CreateExamResponse>(exam);
    }
}
