using MedFlow.Domain.Entities;

namespace MedFlow.Infrastructure.Persistence.Interfaces;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string normalizedEmail);

    Task<User?> GetByNormalizedEmailWithRolesAsync(string normalizedEmail);
}
