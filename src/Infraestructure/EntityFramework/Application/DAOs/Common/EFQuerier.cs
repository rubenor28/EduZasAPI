using Application.DAOs;
using Application.DTOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Common;

public abstract class EFQuerier<DomainEntity, EntityCriteria, EFEntity>(
    EduZasDotnetContext ctx,
    IEFProjector<EFEntity, DomainEntity> projector,
    int pageSize
)
    : EntityFrameworkDAO<DomainEntity, EFEntity>(ctx, projector),
        IQuerierAsync<DomainEntity, EntityCriteria>
    where EFEntity : class
    where EntityCriteria : CriteriaDTO
    where DomainEntity : notnull
{
    protected readonly int _pageSize = pageSize;
    private readonly IEFProjector<EFEntity, DomainEntity> _projector = projector;

    public int PageSize => _pageSize;

    protected int CalcOffset(int pageNumber)
    {
        if (pageNumber < 1)
            pageNumber = 1;
        return (pageNumber - 1) * _pageSize;
    }

    public async Task<PaginatedQuery<DomainEntity, EntityCriteria>> GetByAsync(
        EntityCriteria criteria
    )
    {
        var query = BuildQuery(criteria);
        var totalRecords = await query.CountAsync();

        var results = await query
            .Select(_projector.Projection)
            .Skip(CalcOffset(criteria.Page))
            .Take(_pageSize)
            .ToListAsync();

        int totalPages = (int)Math.Ceiling((decimal)totalRecords / _pageSize);

        return new()
        {
            Page = criteria.Page,
            TotalPages = totalPages,
            Criteria = criteria,
            Results = results,
        };
    }

    public abstract IQueryable<EFEntity> BuildQuery(EntityCriteria criteria);
}
