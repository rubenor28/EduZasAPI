using Application.DTOs.Common;

namespace Application.DAOs;

/// <summary>
/// Interfaz genérica para consultar entidades de un repositorio usando criterios de filtrado.
/// </summary>
/// <typeparam name="E">Tipo de la entidad que se consultará.</typeparam>
/// <typeparam name="C">Tipo de los criterios de consulta, debe implementar <see cref="ICriteriaDTO"/>.</typeparam>
public interface IQuerierAsync<E, C>
    where E : notnull
    where C : notnull, CriteriaDTO
{
    /// <summary>
    /// Obtiene el tamaño de página configurado para las consultas.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Obtiene entidades que cumplen con los criterios especificados.
    /// </summary>
    /// <param name="query">Criterios de consulta.</param>
    /// <returns>Resultados paginados que cumplen con los criterios.</returns>
    Task<PaginatedQuery<E, C>> GetByAsync(C query);

    /// <summary>
    /// Obtiene la cuenta de resultados de una consulta.
    /// </summary>
    /// <param name="query">Criterios de consulta.</param>
    /// <returns>Numero de resultados de una consulta.</returns>
    Task<int> CountAsync(C query);

    /// <summary>
    /// Indica si hay al menos un resultado en una busqueda realizada.
    /// </summary>
    /// <param name="query">Criterios de consulta.</param>
    /// <returns>Booleano indicando si hay o no resultados.</returns>
    Task<bool> AnyAsync(C query);
}
