using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Common;

/// <summary>
/// Implementación base abstracta de un repositorio utilizando Entity Framework Core.
/// </summary>
/// <typeparam name="I">Tipo del identificador de entidad. Debe ser no nulo.</typeparam>
/// <typeparam name="E">Tipo de la entidad de dominio. Debe ser no nulo e implementar <see cref="IIdentifiable{I}"/>.</typeparam>
/// <typeparam name="NE">Tipo del DTO para crear nuevas entidades. Debe ser no nulo.</typeparam>
/// <typeparam name="UE">Tipo del DTO para actualizar entidades. Debe ser no nulo.</typeparam>
/// <typeparam name="C">Tipo de los criterios de búsqueda. Debe ser no nulo e implementar <see cref="ICriteriaDTO"/>.</typeparam>
/// <typeparam name="TEF">Tipo de la entidad de Entity Framework.</typeparam>
/// <remarks>
/// Esta clase abstracta proporciona una implementación base para repositorios que utilizan
/// Entity Framework Core, manejando las operaciones CRUD básicas y la paginación.
/// </remarks>
public abstract class EntityFrameworkRepository<I, E, NE, UE, C, TEF> : IRepositoryAsync<I, E, NE, UE, C>
where I : notnull
where E : notnull, IIdentifiable<I>
where NE : notnull
where UE : notnull
where C : notnull, ICriteriaDTO
where TEF : class
{
    /// <summary>
    /// Contexto de Entity Framework utilizado para acceder a la base de datos.
    /// </summary>
    protected readonly EduZasDotnetContext _ctx;

    /// <summary>
    /// Tamaño de página utilizado para la paginación de resultados.
    /// </summary>
    protected readonly ulong _pageSize;

    /// <summary>
    /// Obtiene el DbSet de Entity Framework para el tipo TEF.
    /// </summary>
    protected DbSet<TEF> DbSet => _ctx.Set<TEF>();

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="EntityFrameworkRepository{I, E, NE, UE, C, TEF}"/>.
    /// </summary>
    /// <param name="context">Contexto de Entity Framework.</param>
    /// <param name="pageSize">Tamaño de página para la paginación.</param>
    public EntityFrameworkRepository(EduZasDotnetContext context, ulong pageSize)
    {
        _ctx = context;
        _pageSize = pageSize;
    }

    /// <summary>
    /// Calcula el offset para la paginación basado en el número de página.
    /// </summary>
    /// <param name="pageNumber">Número de página (comienza en 1).</param>
    /// <returns>Offset calculado para la consulta.</returns>
    protected ulong CalcOffset(ulong pageNumber)
    {
        if (pageNumber < 1) pageNumber = 1;
        return (pageNumber - 1) * _pageSize;
    }


    /// <inheritdoc/>
    public async Task<E> AddAsync(NE data)
    {
        var entity = NewToEF(data);
        await DbSet.AddAsync(entity);
        await _ctx.SaveChangesAsync();
        return MapToDomain(entity);
    }

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

    /// <inheritdoc/>
    public async Task<Optional<E>> GetAsync(I id)
    {
        var record = await DbSet.FindAsync(id);
        if (record is null) return Optional<E>.None();
        return Optional<E>.Some(MapToDomain(record));
    }

    /// <inheritdoc/>
    public async Task<Optional<E>> DeleteAsync(I id)
    {
        var record = await DbSet.FindAsync(id);
        if (record is null) return Optional<E>.None();

        DbSet.Remove(record);
        await _ctx.SaveChangesAsync();
        return Optional<E>.Some(MapToDomain(record));
    }

    /// <inheritdoc/>
    public async Task<PaginatedQuery<E, C>> GetByAsync(C criteria)
    {
        var query = QueryFromCriteria(criteria);
        var totalRecords = (ulong)await query.CountAsync();
        var rawResults = await query
          .Skip((int)CalcOffset(criteria.Page))
          .Take((int)_pageSize)
          .ToListAsync();

        ulong totalPages = (ulong)Math.Ceiling((decimal)totalRecords / (decimal)_pageSize);

        return new PaginatedQuery<E, C>
        {
            Page = criteria.Page,
            TotalPages = totalPages,
            Criteria = criteria,
            Results = rawResults.Select(MapToDomain).ToList()
        };
    }

    /// <summary>
    /// Construye una consulta IQueryable basada en los criterios de búsqueda especificados.
    /// </summary>
    /// <param name="criteria">Criterios de búsqueda para aplicar a la consulta.</param>
    /// <returns>Una consulta IQueryable configurada con los criterios aplicados.</returns>
    protected abstract IQueryable<TEF> QueryFromCriteria(C criteria);

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
    /// Mapea una entidad de Entity Framework a una entidad de dominio.
    /// </summary>
    /// <param name="efEntity">Entidad de Entity Framework.</param>
    /// <returns>Entidad de dominio mapeada.</returns>
    protected abstract E MapToDomain(TEF efEntity);

    /// <summary>
    /// Mapea un DTO de nueva entidad a una entidad de Entity Framework.
    /// </summary>
    /// <param name="newEntity">DTO de nueva entidad.</param>
    /// <returns>Entidad de Entity Framework mapeada.</returns>
    protected abstract TEF NewToEF(NE newEntity);

    /// <summary>
    /// Actualiza las propiedades de una entidad de Entity Framework con los datos de un DTO de actualización.
    /// </summary>
    /// <param name="entity">Entidad de Entity Framework a actualizar.</param>
    /// <param name="updateProperties">DTO con las propiedades actualizadas.</param>
    protected abstract void UpdateProperties(TEF entity, UE updateProperties);
}
