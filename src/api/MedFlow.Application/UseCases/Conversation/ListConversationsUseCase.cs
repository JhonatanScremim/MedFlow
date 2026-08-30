using MedFlow.Application.Contracts.Conversation;
using MedFlow.Application.Exceptions;
using MedFlow.Application.UseCases.Conversation.Interfaces;
using MedFlow.Infrastructure.Persistence.Interfaces;

namespace MedFlow.Application.UseCases.Conversation;

public sealed class ListConversationsUseCase(
    IConversationRepository conversationRepository) : IListConversationsUseCase
{
    public async Task<IReadOnlyList<ConversationResponse>> ExecuteAsync(Guid? patientId, Guid? doctorId)
    {
        if (patientId is null && doctorId is null)
            throw new AuthenticationException("Token não possui perfil de paciente ou médico.");

        var conversations = doctorId is not null
            ? await conversationRepository.GetForDoctorAsync(doctorId.Value)
            : await conversationRepository.GetForPatientAsync(patientId!.Value);

        return conversations
            .Select(conversation => new ConversationResponse(
                conversation.Id,
                conversation.ExamId,
                conversation.Exam.PatientId,
                conversation.Exam.DoctorId,
                conversation.CreatedAt))
            .ToList();
    }
}
