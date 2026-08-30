using AutoMapper;
using MedFlow.Application.Contracts.Exam;
using MedFlow.Application.Exceptions;
using MedFlow.Application.Realtime;
using MedFlow.Application.UseCases.Exam.Interfaces;
using MedFlow.Domain.Enums;
using MedFlow.Infrastructure.Persistence.Interfaces;

namespace MedFlow.Application.UseCases.Exam;

public sealed class UpdateExamStatusUseCase(
    IExamRepository examRepository,
    IMapper mapper,
    INotificationService notificationService) : IUpdateExamStatusUseCase
{
    public async Task<ExamResponse> ExecuteAsync(Guid doctorId, Guid examId, UpdateExamStatusRequest request)
    {
        if (!Enum.IsDefined(typeof(ExamStatus), request.Status))
            throw new BadRequestException("Status de exame inválido.");

        var exam = await examRepository.GetByIdWithConversationAsync(examId);
        if (exam is null)
            throw new NotFoundException("Exame não encontrado.");

        if (exam.DoctorId is not null && exam.DoctorId != doctorId)
            throw new ForbiddenException("Este exame já está atribuído a outro médico.");

        exam.DoctorId ??= doctorId;
        exam.Status = request.Status;
        exam.UpdatedAt = DateTimeOffset.UtcNow;

        await examRepository.UpdateAsync(exam);

        var response = mapper.Map<ExamResponse>(exam);
        await notificationService.NotifyExamUpdatedAsync(response);

        return response;
    }
}
