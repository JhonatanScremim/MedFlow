namespace MedFlow.Domain.Entities;

/// <summary>Entidade com chave primária <see cref="Guid"/> (não usar em tipos com chave composta, ex.: <see cref="UserRole"/>).</summary>
public interface IEntity
{
    Guid Id { get; }
}
