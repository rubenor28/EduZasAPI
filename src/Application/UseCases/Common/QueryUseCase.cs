using Application.DAOs;
using Application.DTOs.Common;

namespace Application.UseCases.Common;

/// <summary>
/// Caso de uso genérico para la ejecución de consultas paginadas
/// basado en criterios de búsqueda.
/// </summary>
/// <typeparam name="E">Tipo de la entidad resultante de la consulta.</typeparam>
/// <typeparam name="C">Tipo del criterio de búsqueda, debe implementar <see cref="ICriteriaDTO"/>.</typeparam>
public class QueryUseCase<C, E>(IQuerierAsync<E, C> querier)
    : IGuaranteedUseCaseAsync<C, PaginatedQuery<E, C>>
    where C : notnull, CriteriaDTO
    where E : notnull
{
    protected IQuerierAsync<E, C> _querier = querier;

    /// <summary>
    /// Ejecuta la consulta asíncrona con los criterios especificados.
    /// </summary>
    /// <param name="criteria">Criterios de búsqueda.</param>
    /// <returns>
    /// Una consulta paginada de tipo <see cref="PaginatedQuery{E, C}"/>
    /// con los resultados correspondientes.
    /// </returns>
    public Task<PaginatedQuery<E, C>> ExecuteAsync(C criteria) => _querier.GetByAsync(criteria);
}
