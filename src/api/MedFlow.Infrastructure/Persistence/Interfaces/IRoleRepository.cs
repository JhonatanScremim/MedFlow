using MedFlow.Domain.Entities;

namespace MedFlow.Infrastructure.Persistence.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
}
