namespace MedFlow.Domain.Entities;

public class Message : IEntity
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid? SenderUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset SentAt { get; set; }

    public Conversation Conversation { get; set; } = null!;
    public User? Sender { get; set; }
}
