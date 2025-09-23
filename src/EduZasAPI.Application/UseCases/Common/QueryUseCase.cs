namespace EduZasAPI.Application.Common;

/// <summary>
/// Caso de uso genérico para la ejecución de consultas paginadas 
/// basado en criterios de búsqueda.
/// </summary>
/// <typeparam name="E">Tipo de la entidad resultante de la consulta.</typeparam>
/// <typeparam name="C">Tipo del criterio de búsqueda, debe implementar <see cref="ICriteriaDTO"/>.</typeparam>
public class QueryUseCase<C, E> : IUseCaseAsync<C, PaginatedQuery<E, C>>
    where C : notnull, ICriteriaDTO
    where E : notnull
{
    private readonly IQuerierAsync<E, C> _querier;

    /// <summary>
    /// Inicializa una nueva instancia del caso de uso de consulta.
    /// </summary>
    /// <param name="querier">Componente encargado de ejecutar la consulta asíncrona.</param>
    public QueryUseCase(IQuerierAsync<E, C> querier)
    {
        _querier = querier;
    }

    /// <summary>
    /// Ejecuta la consulta asíncrona con los criterios especificados.
    /// </summary>
    /// <param name="criteria">Criterios de búsqueda.</param>
    /// <returns>
    /// Una consulta paginada de tipo <see cref="PaginatedQuery{E, C}"/> 
    /// con los resultados correspondientes.
    /// </returns>
    public async Task<PaginatedQuery<E, C>> ExecuteAsync(C criteria)
    {
        return await _querier.GetByAsync(criteria);
    }
}
