using Application.DAOs;
using Application.DTOs.ClassContent;
using Application.DTOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassContent;

public sealed class ClassContentEFQuerier(EduZasDotnetContext ctx, int maxPageSize)
    : IQuerierAsync<ClassContentDTO, ClassContentCriteriaDTO>
{
    private readonly EduZasDotnetContext _ctx = ctx;
    private readonly int _maxPageSize = maxPageSize;

    public int PageSize => _maxPageSize;

    public async Task<bool> AnyAsync(ClassContentCriteriaDTO query)
    {
        var combined = BuildCombinedQuery(query);
        return await combined.AnyAsync();
    }

    public async Task<int> CountAsync(ClassContentCriteriaDTO query)
    {
        var combined = BuildCombinedQuery(query);
        return await combined.CountAsync();
    }

    public async Task<PaginatedQuery<ClassContentDTO, ClassContentCriteriaDTO>> GetByAsync(
        ClassContentCriteriaDTO query
    )
    {
        var combined = BuildCombinedQuery(query);

        var totalRecords = await combined.CountAsync();

        var pageSize = query.PageSize < _maxPageSize ? query.PageSize : _maxPageSize;
        if (pageSize <= 0)
            pageSize = _maxPageSize;

        var pageNumber = query.Page;
        if (pageNumber < 1)
            pageNumber = 1;

        var offset = (pageNumber - 1) * pageSize;

        var results = await combined
            .OrderByDescending(c => c.PublishDate)
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = pageSize > 0 ? (int)Math.Ceiling((decimal)totalRecords / pageSize) : 0;

        return new PaginatedQuery<ClassContentDTO, ClassContentCriteriaDTO>
        {
            Page = pageNumber,
            TotalPages = totalPages,
            Criteria = query,
            Results = results,
        };
    }

    /// <summary>
    /// Construye la consulta combinada (tests + resources) ya proyectada a <see cref="ClassContentDTO"/>.
    /// </summary>
    private IQueryable<ClassContentDTO> BuildCombinedQuery(ClassContentCriteriaDTO criteria)
    {
        // Query para tests asociados a la clase
        var testsQuery = _ctx
            .TestsPerClasses.AsNoTracking()
            .Where(tpc => tpc.ClassId == criteria.ClassId)
            .WhereStringQuery(criteria.Title, tpc => tpc.Test.Title)
            .Select(tpc => new ClassContentDTO
            {
                Id = tpc.Test.TestId,
                Title = tpc.Test.Title,
                Type = ContentType.TEST,
                Hidden = null,
                PublishDate = tpc.CreatedAt,
            });

        // Query para recursos asociados a la clase
        var resourcesQuery = _ctx
            .ClassResources.AsNoTracking()
            .Where(cr => cr.ClassId == criteria.ClassId)
            .WhereOptional(criteria.Visible, v => cr => !cr.Hidden == v)
            .WhereStringQuery(criteria.Title, cr => cr.Resource.Title)
            .Select(cr => new ClassContentDTO
            {
                Id = cr.Resource.ResourceId,
                Title = cr.Resource.Title,
                Type = ContentType.RESOURCE,
                Hidden = cr.Hidden,
                PublishDate = cr.CreatedAt,
            });

        if (criteria.Type is not null && criteria.Type == ContentType.TEST)
            return testsQuery;
        if (criteria.Type is not null && criteria.Type == ContentType.RESOURCE)
            return resourcesQuery;
        return testsQuery.Concat(resourcesQuery);
    }
}
