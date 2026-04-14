using MedFlow.Infrastructure.Persistence.DbContexts;
using MedFlow.Infrastructure.Persistence.Interfaces;
using MedFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedFlow.Infrastructure.Persistence;

public sealed class UserRepository(MedFlowDbContext db) : IUserRepository
{
    public Task<bool> EmailExistsAsync(string normalizedEmail) =>
        db.Users.AnyAsync(u => u.Email == normalizedEmail);

    public Task<User?> GetByNormalizedEmailWithRolesAsync(string normalizedEmail) =>
        db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.Doctor)
            .Include(u => u.Patient)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);
}
