using Application.DAOs;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Common;

/// <summary>
/// Implementación base para consultar entidades usando EF.
/// </summary>
/// <typeparam name="DomainEntity">Entidad de dominio.</typeparam>
/// <typeparam name="EntityCriteria">Criterios de búsqueda.</typeparam>
/// <typeparam name="EFEntity">Entidad de EF.</typeparam>
public abstract class EFQuerier<DomainEntity, EntityCriteria, EFEntity>(
    EduZasDotnetContext ctx,
    IEFProjector<EFEntity, DomainEntity, EntityCriteria> projector,
    int maxPageSize
)
    : EntityFrameworkDAO<DomainEntity, EFEntity>(ctx),
        IQuerierAsync<DomainEntity, EntityCriteria>
    where EFEntity : class
    where EntityCriteria : CriteriaDTO
    where DomainEntity : notnull
{
    protected readonly int _maxPageSize = maxPageSize;
    private readonly IEFProjector<EFEntity, DomainEntity, EntityCriteria> _projector = projector;

    /// <summary>
    /// Tamaño máximo de página permitido.
    /// </summary>
    public int PageSize => _maxPageSize;

    /// <summary>
    /// Realiza una consulta paginada a partir de los criterios de búsqueda.
    /// </summary>
    public async Task<PaginatedQuery<DomainEntity, EntityCriteria>> GetByAsync(
        EntityCriteria criteria
    )
    {
        var query = BuildQuery(criteria).AsNoTracking();
        var totalRecords = await query.CountAsync();

        var pageSize = criteria.PageSize < _maxPageSize ? criteria.PageSize : _maxPageSize;
        if (pageSize <= 0)
            pageSize = _maxPageSize;

        var pageNumber = criteria.Page;
        if (pageNumber < 1)
            pageNumber = 1;

        var offset = (pageNumber - 1) * pageSize;

        var results = await query
            .Select(_projector.GetProjection(criteria))
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = pageSize > 0 ? (int)Math.Ceiling((decimal)totalRecords / pageSize) : 0;

        return new()
        {
            Page = criteria.Page,
            TotalPages = totalPages,
            Criteria = criteria with { PageSize = pageSize, Page = pageNumber },
            Results = results,
        };
    }

    /// <summary>
    /// Cuenta el número total de entidades que coinciden con los criterios.
    /// </summary>
    public Task<int> CountAsync(EntityCriteria criteria) => BuildQuery(criteria).AsNoTracking().CountAsync();

    /// <summary>
    /// Verifica si existe alguna entidad que coincida con los criterios.
    /// </summary>
    public Task<bool> AnyAsync(EntityCriteria criteria) => BuildQuery(criteria).AsNoTracking().AnyAsync();

    /// <summary>
    /// Construye la consulta base a partir de los criterios.
    /// </summary>
    public abstract IQueryable<EFEntity> BuildQuery(EntityCriteria criteria);
}
