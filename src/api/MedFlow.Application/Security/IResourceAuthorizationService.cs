namespace MedFlow.Application.Security;

public interface IResourceAuthorizationService
{
    Task<bool> CanAccessExamAsync(Guid examId, Guid? patientId, Guid? doctorId);

    Task<bool> CanAccessConversationAsync(Guid conversationId, Guid? patientId, Guid? doctorId);
}
