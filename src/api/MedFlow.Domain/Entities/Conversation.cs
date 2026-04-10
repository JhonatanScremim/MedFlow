namespace MedFlow.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Exam Exam { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
