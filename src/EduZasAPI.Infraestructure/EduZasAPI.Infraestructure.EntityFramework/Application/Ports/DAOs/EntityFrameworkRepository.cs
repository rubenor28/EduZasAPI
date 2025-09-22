namespace EduZasAPI.Infraestructure.Application.Ports.DAOs;

using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Application.DTOs.Common;
using EduZasAPI.Application.Ports.DAOs;

using EduZasAPI.Infraestructure.Application.DTOs;

using Microsoft.EntityFrameworkCore;

public abstract class EntityFrameworkRepository<I, E, NE, UE, C, TEF> : IRepositoryAsync<I, E, NE, UE, C>
where I : notnull
where E : notnull, IIdentifiable<I>
where NE : notnull
where UE : notnull
where C : notnull, ICriteriaDTO
where TEF : class
{
    protected readonly EduZasDotnetContext _ctx;
    protected readonly ulong _pageSize;
    protected DbSet<TEF> DbSet => _ctx.Set<TEF>();

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



    public async Task<E> AddAsync(NE data)
    {
        var entity = NewToEF(data);
        await DbSet.AddAsync(entity);
        await _ctx.SaveChangesAsync();
        return MapToDomain(entity);
    }


    public async Task<E> UpdateAsync(UE updateData)
    {
        var id = GetId(updateData);
        var tracked = await DbSet.FindAsync(id);

        if (tracked == null) throw new ArgumentException($"Entity with id {id} not found");

        UpdateProperties(tracked, updateData);

        DbSet.Update(tracked);
        await _ctx.SaveChangesAsync();
        return MapToDomain(tracked);
    }

    public async Task<Optional<E>> GetAsync(I id)
    {
        var record = await DbSet.FindAsync(id);
        if (record is null) return Optional<E>.None();
        return Optional<E>.Some(MapToDomain(record));
    }

    public async Task<Optional<E>> DeleteAsync(I id)
    {
        var record = await DbSet.FindAsync(id);
        if (record is null) return Optional<E>.None();

        DbSet.Remove(record);
        await _ctx.SaveChangesAsync();
        return Optional<E>.Some(MapToDomain(record));
    }

    protected async Task<PaginatedQuery<E, C>> ExecuteQuery(IQueryable<TEF> query, C criteria)
    {
        var totalRecords = (ulong)await query.CountAsync();
        var queryResults = await query
          .OrderBy(data => GetId(data))
          .Skip((int)CalcOffset(criteria.Page))
          .Take((int)_pageSize)
          .Select(data => MapToDomain(data))
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

    protected abstract I GetId(TEF entity);
    protected abstract I GetId(UE entity);
    protected abstract E MapToDomain(TEF efEntity);
    protected abstract TEF NewToEF(NE newEntity);
    protected abstract void UpdateProperties(TEF entity, UE updateProperties);
}
