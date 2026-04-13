namespace MedFlow.Domain.Entities;

public class User : IEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}
