using MedFlow.Domain.Entities;

namespace MedFlow.Infrastructure.Persistence.Interfaces;

public interface IConversationRepository
{
    Task<IReadOnlyList<Conversation>> GetForPatientAsync(Guid patientId);

    Task<IReadOnlyList<Conversation>> GetForDoctorAsync(Guid doctorId);

    Task<Conversation?> GetByIdWithExamAsync(Guid id);

    Task<IReadOnlyList<Message>> GetMessagesAsync(Guid conversationId, int skip, int take);

    Task<Message> CreateMessageAsync(Message message);
}
