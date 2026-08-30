using MedFlow.Domain.Entities;

namespace MedFlow.Infrastructure.Persistence.Interfaces;

public interface IExamRepository
{
    Task<IReadOnlyList<Exam>> GetForPatientAsync(Guid patientId);

    Task<IReadOnlyList<Exam>> GetForDoctorDashboardAsync(Guid doctorId);

    Task<Exam?> GetByIdWithConversationAsync(Guid id);

    Task UpdateAsync(Exam exam);
}
