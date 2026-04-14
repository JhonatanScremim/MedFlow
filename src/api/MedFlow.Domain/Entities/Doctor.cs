namespace MedFlow.Domain.Entities;

public class Doctor : IEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
