using MedFlow.Domain.Entities;
using MedFlow.Infrastructure.Persistence.DbContexts;
using MedFlow.Infrastructure.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedFlow.Infrastructure.Persistence;

public sealed class ConversationRepository(MedFlowDbContext db) : IConversationRepository
{
    public async Task<IReadOnlyList<Conversation>> GetForPatientAsync(Guid patientId) =>
        await db.Conversations
            .AsNoTracking()
            .Include(conversation => conversation.Exam)
            .Where(conversation => conversation.Exam.PatientId == patientId)
            .OrderByDescending(conversation => conversation.CreatedAt)
            .ToListAsync();

    public async Task<IReadOnlyList<Conversation>> GetForDoctorAsync(Guid doctorId) =>
        await db.Conversations
            .AsNoTracking()
            .Include(conversation => conversation.Exam)
            .Where(conversation => conversation.Exam.DoctorId == doctorId)
            .OrderByDescending(conversation => conversation.CreatedAt)
            .ToListAsync();

    public Task<Conversation?> GetByIdWithExamAsync(Guid id) =>
        db.Conversations
            .AsNoTracking()
            .Include(conversation => conversation.Exam)
            .FirstOrDefaultAsync(conversation => conversation.Id == id);

    public async Task<IReadOnlyList<Message>> GetMessagesAsync(Guid conversationId, int skip, int take) =>
        await db.Messages
            .AsNoTracking()
            .Where(message => message.ConversationId == conversationId)
            .OrderByDescending(message => message.SentAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

    public async Task<Message> CreateMessageAsync(Message message)
    {
        await db.Messages.AddAsync(message);
        await db.SaveChangesAsync();
        return message;
    }
}
