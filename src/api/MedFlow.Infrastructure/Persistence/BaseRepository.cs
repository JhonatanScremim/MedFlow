using MedFlow.Infrastructure.Persistence.DbContexts;
using MedFlow.Infrastructure.Persistence.Interfaces;
using MedFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedFlow.Infrastructure.Persistence;

public class BaseRepository<TEntity>(MedFlowDbContext db) : IBaseRepository<TEntity>
    where TEntity : class, IEntity
{
    protected MedFlowDbContext Db => db;

    protected DbSet<TEntity> Set => db.Set<TEntity>();

    public Task<TEntity?> GetByIdAsync(Guid id) =>
        Set.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync() =>
        await Set.AsNoTracking().ToListAsync();

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        await Set.AddAsync(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        Set.Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        Set.Remove(entity);
        await db.SaveChangesAsync();
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        var entity = await Set.FindAsync([id]);
        if (entity is null)
            return false;

        Set.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}
