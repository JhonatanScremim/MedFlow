using MedFlow.Domain.Entities;
using MedFlow.Domain.Enums;
using MedFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedFlow.Infrastructure.Tests;

public class MedFlowDbContextTests : IAsyncLifetime
{
    private readonly string _databaseName = $"MedFlowTests_{Guid.NewGuid():N}";
    private readonly string _connectionString;

    public MedFlowDbContextTests()
    {
        _connectionString =
            $"Server=localhost;Database={_databaseName};Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
    }

    [Fact]
    public async Task Should_Insert_And_Read_Exam_With_Type()
    {
        await using var arrangeContext = CreateContext();
        var patient = await CreatePatientAsync(arrangeContext);

        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            PatientId = patient.Id,
            Type = ExamType.BloodTest,
            Status = ExamStatus.Requested,
            Notes = "Coleta em jejum",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        arrangeContext.Exams.Add(exam);
        await arrangeContext.SaveChangesAsync();

        await using var assertContext = CreateContext();
        var savedExam = await assertContext.Exams
            .AsNoTracking()
            .SingleAsync(x => x.Id == exam.Id);

        Assert.Equal(ExamType.BloodTest, savedExam.Type);
        Assert.Equal(ExamStatus.Requested, savedExam.Status);
        Assert.Equal("Coleta em jejum", savedExam.Notes);
    }

    [Fact]
    public async Task Should_Update_And_Delete_Exam()
    {
        await using var arrangeContext = CreateContext();
        var patient = await CreatePatientAsync(arrangeContext);

        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            PatientId = patient.Id,
            Type = ExamType.XRay,
            Status = ExamStatus.InProgress,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        arrangeContext.Exams.Add(exam);
        await arrangeContext.SaveChangesAsync();

        await using var updateContext = CreateContext();
        var examToUpdate = await updateContext.Exams.SingleAsync(x => x.Id == exam.Id);
        examToUpdate.Status = ExamStatus.Completed;
        examToUpdate.Type = ExamType.ComputedTomography;
        examToUpdate.UpdatedAt = DateTimeOffset.UtcNow;
        await updateContext.SaveChangesAsync();

        await using var deleteContext = CreateContext();
        var examToDelete = await deleteContext.Exams.SingleAsync(x => x.Id == exam.Id);
        deleteContext.Exams.Remove(examToDelete);
        await deleteContext.SaveChangesAsync();

        await using var assertContext = CreateContext();
        var exists = await assertContext.Exams.AnyAsync(x => x.Id == exam.Id);
        Assert.False(exists);
    }

    public async Task InitializeAsync()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
    }

    private MedFlowDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MedFlowDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        return new MedFlowDbContext(options);
    }

    private static async Task<Patient> CreatePatientAsync(MedFlowDbContext context)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = $"patient-{Guid.NewGuid():N}@medflow.local",
            PasswordHash = "hashed-password",
            CreatedAt = DateTimeOffset.UtcNow,
        };

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            IsAnonymous = false,
            CreatedAt = DateTimeOffset.UtcNow,
            User = user,
        };

        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        return patient;
    }
}
