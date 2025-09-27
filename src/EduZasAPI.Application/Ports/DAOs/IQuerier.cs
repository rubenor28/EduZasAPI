namespace EduZasAPI.Application.Common;

/// <summary>
/// Interfaz genérica para consultar entidades de un repositorio usando criterios de filtrado.
/// </summary>
/// <typeparam name="E">Tipo de la entidad que se consultará.</typeparam>
/// <typeparam name="C">Tipo de los criterios de consulta, debe implementar <see cref="ICriteriaDTO"/>.</typeparam>
public interface IQuerierAsync<E, C>
    where E : notnull
    where C : notnull, ICriteriaDTO
{
    /// <summary>
    /// Obtiene entidades que cumplen con los criterios especificados.
    /// </summary>
    /// <param name="query">Criterios de consulta.</param>
    /// <returns>Resultados paginados que cumplen con los criterios.</returns>
    Task<PaginatedQuery<E, C>> GetByAsync(C query);
}
