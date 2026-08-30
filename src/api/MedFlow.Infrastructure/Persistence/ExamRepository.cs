using MedFlow.Domain.Entities;
using MedFlow.Infrastructure.Persistence.DbContexts;
using MedFlow.Infrastructure.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedFlow.Infrastructure.Persistence;

public sealed class ExamRepository(MedFlowDbContext db) : IExamRepository
{
    public async Task<IReadOnlyList<Exam>> GetForPatientAsync(Guid patientId) =>
        await db.Exams
            .AsNoTracking()
            .Include(exam => exam.Conversation)
            .Where(exam => exam.PatientId == patientId)
            .OrderByDescending(exam => exam.CreatedAt)
            .ToListAsync();

    public async Task<IReadOnlyList<Exam>> GetForDoctorDashboardAsync(Guid doctorId) =>
        await db.Exams
            .AsNoTracking()
            .Include(exam => exam.Conversation)
            .Where(exam => exam.DoctorId == null || exam.DoctorId == doctorId)
            .OrderByDescending(exam => exam.CreatedAt)
            .ToListAsync();

    public Task<Exam?> GetByIdWithConversationAsync(Guid id) =>
        db.Exams
            .Include(exam => exam.Conversation)
            .FirstOrDefaultAsync(exam => exam.Id == id);

    public async Task UpdateAsync(Exam exam)
    {
        db.Exams.Update(exam);
        await db.SaveChangesAsync();
    }
}
