using MedFlow.Infrastructure.Persistence.Interfaces;

namespace MedFlow.Application.Security;

public sealed class ResourceAuthorizationService(
    IExamRepository examRepository,
    IConversationRepository conversationRepository) : IResourceAuthorizationService
{
    public async Task<bool> CanAccessExamAsync(Guid examId, Guid? patientId, Guid? doctorId)
    {
        var exam = await examRepository.GetByIdWithConversationAsync(examId);
        if (exam is null)
            return false;

        if (patientId is not null && exam.PatientId == patientId)
            return true;

        return doctorId is not null && (exam.DoctorId is null || exam.DoctorId == doctorId);
    }

    public async Task<bool> CanAccessConversationAsync(Guid conversationId, Guid? patientId, Guid? doctorId)
    {
        var conversation = await conversationRepository.GetByIdWithExamAsync(conversationId);
        if (conversation is null)
            return false;

        if (patientId is not null && conversation.Exam.PatientId == patientId)
            return true;

        return doctorId is not null && conversation.Exam.DoctorId == doctorId;
    }
}
