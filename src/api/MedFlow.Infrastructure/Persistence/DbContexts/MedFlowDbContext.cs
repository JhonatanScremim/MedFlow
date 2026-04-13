using MedFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedFlow.Infrastructure.Persistence.DbContexts;

public class MedFlowDbContext(DbContextOptions<MedFlowDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<Role>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(128).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<UserRole>(e =>
        {
            e.HasKey(x => new { x.UserId, x.RoleId });
            e.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
        });

        modelBuilder.Entity<Patient>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserId).IsUnique();
            e.HasOne(x => x.User).WithOne(x => x.Patient).HasForeignKey<Patient>(x => x.UserId);
            e.HasMany(x => x.Exams).WithOne(x => x.Patient).HasForeignKey(x => x.PatientId);
        });

        modelBuilder.Entity<Doctor>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserId).IsUnique();
            e.HasOne(x => x.User).WithOne(x => x.Doctor).HasForeignKey<Doctor>(x => x.UserId);
        });

        modelBuilder.Entity<Exam>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasConversion<int>();
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.HasOne(x => x.Conversation).WithOne(x => x.Exam).HasForeignKey<Conversation>(x => x.ExamId);
        });

        modelBuilder.Entity<Conversation>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ExamId).IsUnique();
            e.HasMany(x => x.Messages).WithOne(x => x.Conversation).HasForeignKey(x => x.ConversationId);
        });

        modelBuilder.Entity<Message>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Content).HasMaxLength(8000).IsRequired();
            e.HasOne(x => x.Sender).WithMany().HasForeignKey(x => x.SenderUserId).OnDelete(DeleteBehavior.NoAction);
        });
    }
}
