namespace EduZasAPI.Infraestructure.Application.Ports.DAOs;

using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Application.DTOs.Common;
using EduZasAPI.Application.Ports.DAOs;
using EduZasAPI.Application.Ports.Mappers;

using EduZasAPI.Infraestructure.Application.DTOs;

using Microsoft.EntityFrameworkCore;

public abstract class EntityFrameworkRepository<I, E, NE, UE, C, TEF> : IRepositoryAsync<I, E, NE, UE, C>
where I : notnull
where E : notnull, IIdentifiable<I>
where NE : notnull
where UE : notnull
where C : notnull, ICriteriaDTO
where TEF : class, IIdentifiable<I>, IInto<E>, IFrom<NE, TEF>, IFrom<UE, TEF>
{
    protected readonly EduZasDotnetContext _ctx;
    protected readonly ulong _pageSize;

    public EntityFrameworkRepository(EduZasDotnetContext context, ulong pageSize)
    {
        _ctx = context;
        _pageSize = pageSize;
    }

    protected ulong CalcOffset(ulong pageNumber)
    {
        if (pageNumber < 1) pageNumber = 1;
        return (pageNumber - 1) * _pageSize;
    }

    protected DbSet<TEF> DbSet => _ctx.Set<TEF>();


    public async Task<E> AddAsync(NE data)
    {
        var entity = TEF.From(data);
        await DbSet.AddAsync(entity);
        await _ctx.SaveChangesAsync();
        return entity.Into();
    }


    public async Task<E> UpdateAsync(UE updateData)
    {
        var entity = TEF.From(updateData);
        DbSet.Update(entity);
        await _ctx.SaveChangesAsync();
        return entity.Into();
    }

    public async Task<Optional<E>> GetAsync(I id)
    {
        var record = await DbSet.FirstOrDefaultAsync(data => data.Id.Equals(id));
        if (record is null) return Optional<E>.None();
        return Optional<E>.Some(record.Into());
    }

    public abstract Task<Optional<E>> DeleteAsync(I id);

    protected async Task<PaginatedQuery<E, C>> ExecuteQuery(IQueryable<TEF> query, C criteria)
    {
        var totalRecords = (ulong)await query.CountAsync();
        var queryResults = await query
          .OrderBy(data => data.Id)
          .Skip((int)CalcOffset(criteria.Page))
          .Take((int)_pageSize)
          .Select(data => data.Into())
          .ToListAsync();

        ulong totalPages = (ulong)Math.Ceiling((decimal)totalRecords / (decimal)_pageSize);

        return new PaginatedQuery<E, C>
        {
            Page = criteria.Page,
            TotalPages = totalPages,
            Criteria = criteria,
            Results = queryResults
        };
    }


    public abstract Task<PaginatedQuery<E, C>> GetByAsync(C query);
}
