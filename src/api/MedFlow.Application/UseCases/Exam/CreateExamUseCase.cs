using MedFlow.Application.Contracts.Exam;
using MedFlow.Application.Exceptions;
using MedFlow.Application.Realtime;
using MedFlow.Application.UseCases.Exam.Interfaces;
using AutoMapper;
using MedFlow.Domain.Entities;
using MedFlow.Domain.Enums;
using MedFlow.Infrastructure.Persistence.Interfaces;
using ConversationEntity = MedFlow.Domain.Entities.Conversation;
using ExamEntity = MedFlow.Domain.Entities.Exam;

namespace MedFlow.Application.UseCases.Exam;

public sealed class CreateExamUseCase(
    IBaseRepository<ExamEntity> examRepository,
    IBaseRepository<ConversationEntity> conversationRepository,
    IBaseRepository<Doctor> doctorRepository,
    IMapper mapper,
    INotificationService notificationService) : ICreateExamUseCase
{
    public async Task<CreateExamResponse> ExecuteAsync(Guid patientId, CreateExamRequest request)
    {
        if (!Enum.IsDefined(typeof(ExamType), request.Type))
            throw new BadRequestException("Tipo de exame inválido.");

        if (request.DoctorId is not null && await doctorRepository.GetByIdAsync(request.DoctorId.Value) is null)
            throw new BadRequestException("Médico informado não foi encontrado.");

        var now = DateTimeOffset.UtcNow;
        var exam = mapper.Map<ExamEntity>(request, options =>
        {
            options.Items["PatientId"] = patientId;
            options.Items["Now"] = now;
        });

        await examRepository.CreateAsync(exam);

        var conversation = await conversationRepository.CreateAsync(new ConversationEntity
        {
            Id = Guid.NewGuid(),
            ExamId = exam.Id,
            CreatedAt = now
        });
        exam.Conversation = conversation;

        var response = mapper.Map<CreateExamResponse>(exam);
        await notificationService.NotifyExamCreatedAsync(mapper.Map<ExamResponse>(exam));

        return response;
    }
}
