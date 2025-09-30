using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Common;

public abstract class CompositeKeyEFRepository<I, E, C, TEF> :
  EntityFrameworkRepository<E, E, C, TEF>, IRepositoryAsync<I, E, E, E, C>
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
        var tracked = await GetTrackedById(id);
        if (tracked == null) throw new ArgumentException($"Entity with id {id} not found");

        UpdateProperties(tracked, updateData);

        DbSet.Update(tracked);
        await _ctx.SaveChangesAsync();
        return MapToDomain(tracked);
    }

    /// <inheritdoc/>
    public async Task<Optional<E>> GetAsync(I id)
    {
        var record = await GetTrackedById(id);
        if (record is null) return Optional<E>.None();
        return Optional<E>.Some(MapToDomain(record));
    }

    /// <inheritdoc/>
    public async Task<Optional<E>> DeleteAsync(I id)
    {
        var record = await GetTrackedById(id);
        if (record is null) return Optional<E>.None();

        DbSet.Remove(record);
        await _ctx.SaveChangesAsync();
        return Optional<E>.Some(MapToDomain(record));
    }

    protected abstract I GetId(E entity);

    protected abstract TEF NewRelationEntityToEF(E newEntity);

    protected abstract Task<TEF> GetTrackedById(I id);

    protected abstract void UpdateProperties(TEF entity, E updateProperties);

}
