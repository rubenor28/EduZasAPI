using Application.DAOs;
using Application.DTOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Common;

public abstract class EFQuerier<DomainEntity, EntityCriteria, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper,
    int pageSize
)
    : EntityFrameworkDAO<DomainEntity, EFEntity>(ctx, domainMapper),
        IQuerierAsync<DomainEntity, EntityCriteria>
    where EFEntity : class
    where EntityCriteria : CriteriaDTO
    where DomainEntity : notnull
{
    protected readonly int _pageSize = pageSize;

    public int PageSize => _pageSize;

    /// <summary>
    /// Calcula el offset para la paginación basado en el número de página.
    /// </summary>
    /// <param name="pageNumber">Número de página (comienza en 1).</param>
    /// <returns>Offset calculado para la consulta.</returns>
    protected int CalcOffset(int pageNumber)
    {
        if (pageNumber < 1)
            pageNumber = 1;
        return (pageNumber - 1) * _pageSize;
    }

    /// <summary>
    /// Obtiene entidades basadas en criterios de búsqueda específicos con paginación.
    /// </summary>
    /// <param name="criteria">Criterios de búsqueda para filtrar las entidades.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado contiene una consulta paginada.</returns>
    public async Task<PaginatedQuery<DomainEntity, EntityCriteria>> GetByAsync(
        EntityCriteria criteria
    )
    {
        var query = BuildQuery(criteria);
        var totalRecords = await query.CountAsync();
        var rawResults = await query.Skip(CalcOffset(criteria.Page)).Take(_pageSize).ToListAsync();

        int totalPages = (int)Math.Ceiling((decimal)totalRecords / _pageSize);

        return new()
        {
            Page = criteria.Page,
            TotalPages = totalPages,
            Criteria = criteria,
            Results = rawResults.Select(_domainMapper.Map),
        };
    }

    public abstract IQueryable<EFEntity> BuildQuery(EntityCriteria criteria);
}
