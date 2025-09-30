using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Domain.Common;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Common;

/// <summary>
/// Implementación base abstracta de un repositorio para entidades con clave primaria simple utilizando Entity Framework Core.
/// </summary>
/// <typeparam name="I">Tipo del identificador de entidad. Debe ser no nulo.</typeparam>
/// <typeparam name="E">Tipo de la entidad de dominio. Debe ser no nulo e implementar <see cref="IIdentifiable{I}"/>.</typeparam>
/// <typeparam name="NE">Tipo del DTO para crear nuevas entidades. Debe ser no nulo.</typeparam>
/// <typeparam name="UE">Tipo del DTO para actualizar entidades. Debe ser no nulo.</typeparam>
/// <typeparam name="C">Tipo de los criterios de búsqueda. Debe ser no nulo e implementar <see cref="ICriteriaDTO"/>.</typeparam>
/// <typeparam name="TEF">Tipo de la entidad de Entity Framework.</typeparam>
public abstract class SimpleKeyEFRepository<I, E, NE, UE, C, TEF> : EntityFrameworkRepository<NE, E, C, TEF>, IRepositoryAsync<I, E, NE, UE, C>
where I : notnull
where E : notnull, IIdentifiable<I>
where NE : notnull
where UE : notnull
where C : notnull, ICriteriaDTO
where TEF : class
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="SimpleKeyEFRepository{I, E, NE, UE, C, TEF}"/>.
    /// </summary>
    /// <param name="context">Contexto de Entity Framework.</param>
    /// <param name="pageSize">Tamaño de página para la paginación.</param>
    public SimpleKeyEFRepository(EduZasDotnetContext context, ulong pageSize) : base(context, pageSize) { }

    /// <summary>
    /// Actualiza una entidad existente en el repositorio.
    /// </summary>
    /// <param name="updateData">DTO con los datos actualizados de la entidad.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado contiene la entidad actualizada.</returns>
    /// <exception cref="ArgumentException">Se lanza cuando la entidad no existe.</exception>
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

    /// <summary>
    /// Obtiene una entidad por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la entidad a buscar.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado contiene un Optional con la entidad si fue encontrada.</returns>
    public async Task<Optional<E>> GetAsync(I id)
    {
        var record = await DbSet.FindAsync(id);
        if (record is null) return Optional<E>.None();
        return Optional<E>.Some(MapToDomain(record));
    }

    /// <summary>
    /// Elimina una entidad del repositorio por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la entidad a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado contiene un Optional con la entidad eliminada si existía.</returns>
    public async Task<Optional<E>> DeleteAsync(I id)
    {
        var record = await DbSet.FindAsync(id);
        if (record is null) return Optional<E>.None();

        if (record is ISoftDeletableEF softDeletable)
        {
            softDeletable.Active = false;
        }
        else
        {
            DbSet.Remove(record);
        }

        await _ctx.SaveChangesAsync();
        return Optional<E>.Some(MapToDomain(record));
    }

    /// <summary>
    /// Obtiene el identificador de una entidad de Entity Framework.
    /// </summary>
    /// <param name="entity">Entidad de Entity Framework.</param>
    /// <returns>Identificador de la entidad.</returns>
    protected abstract I GetId(TEF entity);

    /// <summary>
    /// Obtiene el identificador de un DTO de actualización.
    /// </summary>
    /// <param name="entity">DTO de actualización.</param>
    /// <returns>Identificador de la entidad.</returns>
    protected abstract I GetId(UE entity);

    /// <summary>
    /// Actualiza las propiedades de una entidad de Entity Framework con los datos de un DTO de actualización.
    /// </summary>
    /// <param name="entity">Entidad de Entity Framework a actualizar.</param>
    /// <param name="updateProperties">DTO con las propiedades actualizadas.</param>
    protected abstract void UpdateProperties(TEF entity, UE updateProperties);
}
