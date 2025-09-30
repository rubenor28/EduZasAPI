using EduZasAPI.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Common;

/// <summary>
/// Implementación base abstracta de un repositorio utilizando Entity Framework Core.
/// </summary>
/// <typeparam name="NE">Tipo del DTO para crear nuevas entidades. Debe ser no nulo.</typeparam>
/// <typeparam name="E">Tipo de la entidad de dominio. Debe ser no nulo.</typeparam>
/// <typeparam name="C">Tipo de los criterios de búsqueda. Debe ser no nulo e implementar <see cref="ICriteriaDTO"/>.</typeparam>
/// <typeparam name="TEF">Tipo de la entidad de Entity Framework.</typeparam>
/// <remarks>
/// Esta clase abstracta proporciona una implementación base para repositorios que utilizan
/// Entity Framework Core, manejando operaciones básicas de creación y consulta con paginación.
/// </remarks>
public abstract class EntityFrameworkRepository<NE, E, C, TEF>
where NE : notnull
where E : notnull
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
    /// Inicializa una nueva instancia de la clase <see cref="EntityFrameworkRepository{NE, E, C, TEF}"/>.
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

    /// <summary>
    /// Agrega una nueva entidad al repositorio.
    /// </summary>
    /// <param name="data">DTO con los datos para crear la nueva entidad.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado contiene la entidad creada.</returns>
    public async Task<E> AddAsync(NE data)
    {
        var entity = NewToEF(data);
        await DbSet.AddAsync(entity);
        await _ctx.SaveChangesAsync();
        return MapToDomain(entity);
    }

    /// <summary>
    /// Obtiene entidades basadas en criterios de búsqueda específicos con paginación.
    /// </summary>
    /// <param name="criteria">Criterios de búsqueda para filtrar las entidades.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado contiene una consulta paginada.</returns>
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
    /// Construye una consulta IQueryable basada en los criterios de búsqueda especificados.
    /// </summary>
    /// <param name="criteria">Criterios de búsqueda para aplicar a la consulta.</param>
    /// <returns>Una consulta IQueryable configurada con los criterios aplicados.</returns>
    protected abstract IQueryable<TEF> QueryFromCriteria(C criteria);
}
