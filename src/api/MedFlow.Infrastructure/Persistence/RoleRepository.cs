using MedFlow.Infrastructure.Persistence.DbContexts;
using MedFlow.Infrastructure.Persistence.Interfaces;
using MedFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedFlow.Infrastructure.Persistence;

public sealed class RoleRepository(MedFlowDbContext db) : IRoleRepository
{
    public Task<Role?> GetByNameAsync(string name) =>
        db.Roles.FirstOrDefaultAsync(r => r.Name == name);
}
