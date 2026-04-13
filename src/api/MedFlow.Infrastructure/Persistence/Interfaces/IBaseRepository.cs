using MedFlow.Domain.Entities;

namespace MedFlow.Infrastructure.Persistence.Interfaces;

/// <summary>CRUD genérico para entidades com <see cref="IEntity.Id"/> (não serve para chaves compostas).</summary>
public interface IBaseRepository<TEntity>
    where TEntity : class, IEntity
{
    Task<TEntity?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<TEntity>> GetAllAsync();

    Task<TEntity> CreateAsync(TEntity entity);

    Task UpdateAsync(TEntity entity);

    Task DeleteAsync(TEntity entity);

    /// <returns><see langword="true"/> se existia e foi removida.</returns>
    Task<bool> DeleteByIdAsync(Guid id);
}
