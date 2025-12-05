using Application.DAOs;
using Application.DTOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Common;

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

    public int PageSize => _maxPageSize;

    protected int CalcOffset(int pageNumber)
    {
        if (pageNumber < 1)
            pageNumber = 1;
        return (pageNumber - 1) * _maxPageSize;
    }

    ///<inheritdoc>
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
            Criteria = criteria,
            Results = results,
        };
    }

    ///<inheritdoc>
    public Task<int> CountAsync(EntityCriteria criteria) => BuildQuery(criteria).AsNoTracking().CountAsync();

    ///<inheritdoc>
    public Task<bool> AnyAsync(EntityCriteria criteria) => BuildQuery(criteria).AsNoTracking().AnyAsync();

    /// <summary>
    /// Método encargado de contruir un IQueryable<EFEntity> a partir de un criterio.
    /// </summary>
    public abstract IQueryable<EFEntity> BuildQuery(EntityCriteria criteria);
}
