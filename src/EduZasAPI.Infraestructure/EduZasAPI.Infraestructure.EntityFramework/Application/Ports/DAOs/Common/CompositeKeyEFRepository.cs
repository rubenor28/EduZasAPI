using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Common;

public abstract class CompositeKeyEFRepository<I, E, C, TEF> :
  EntityFrameworkRepository<E, E, C, TEF>, IRepositoryAsync<I, E, E, E, E, C>
where I : notnull
where E : notnull, IIdentifiable<I>
where C : notnull, ICriteriaDTO
where TEF : class
{
    public CompositeKeyEFRepository(EduZasDotnetContext context, ulong pageSize) : base(context, pageSize) { }


    /// <inheritdoc/>
    public async Task<E> UpdateAsync(E updateData)
    {
        var id = GetId(updateData);
        var tracked = await GetByIdTracked(id);
        if (tracked == null) throw new ArgumentException($"Entity with id {id} not found");

        UpdateProperties(tracked, updateData);

        await _ctx.SaveChangesAsync();
        return MapToDomain(tracked);
    }

    /// <inheritdoc/>
    public async Task<Optional<E>> GetAsync(I id)
    {
        var record = await GetById(id);
        if (record is null) return Optional<E>.None();
        return Optional<E>.Some(MapToDomain(record));
    }

    /// <inheritdoc/>
    public async Task<E> DeleteAsync(I id)
    {
        var record = await GetByIdTracked(id);
        if (record is null) throw new ArgumentException("Record do not exists");

        DbSet.Remove(record);
        await _ctx.SaveChangesAsync();
        return MapToDomain(record);
    }

    protected abstract I GetId(E entity);

    protected abstract Task<TEF?> GetById(I id);

    protected abstract Task<TEF?> GetByIdTracked(I id);

    protected abstract void UpdateProperties(TEF entity, E updateProperties);

}
