namespace EduZasAPI.Domain.ValueObjects.Common;

/// <summary>
/// Representa el resultado de una consulta paginada con criterios específicos.
/// </summary>
/// <typeparam name="T">Tipo de los elementos en los resultados. Debe ser un tipo no nulo.</typeparam>
/// <typeparam name="C">Tipo de los criterios de consulta. Debe implementar <see cref="ICriteria"/> y ser no nulo.</typeparam>
/// <remarks>
/// Esta estructura inmutable encapsula los resultados de una consulta paginada,
/// incluyendo información de paginación, criterios de búsqueda y los resultados mismos.
/// </remarks>
public readonly struct PaginatedQuery<T, C>
where T : notnull
where C : notnull, ICriteria
{
    /// <summary>
    /// Obtiene el número de página actual de los resultados.
    /// </summary>
    /// <value>Número de página actual (comienza en 1).</value>
    public ulong Page { get; }

    /// <summary>
    /// Obtiene el número total de páginas disponibles.
    /// </summary>
    /// <value>Total de páginas en la consulta.</value>
    public ulong TotalPages { get; }

    /// <summary>
    /// Obtiene los criterios utilizados para la consulta.
    /// </summary>
    /// <value>Instancia de los criterios de búsqueda.</value>
    public C Criteria { get; }

    /// <summary>
    /// Obtiene la lista de resultados de la consulta.
    /// </summary>
    /// <value>Lista de elementos resultantes de la consulta.</value>
    public List<T> Results { get; }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura <see cref="PaginatedQuery{T, C}"/>.
    /// </summary>
    /// <param name="page">Número de página actual.</param>
    /// <param name="totalPages">Número total de páginas.</param>
    /// <param name="criteria">Criterios utilizados para la consulta.</param>
    /// <param name="results">Lista de resultados de la consulta.</param>
    /// <exception cref="ArgumentException">
    /// Se lanza cuando los criterios o resultados son nulos.
    /// </exception>
    public PaginatedQuery(ulong page, ulong totalPages, C criteria, List<T> results)
    {
        if (criteria == null) throw new ArgumentException("Criteria can not be null", nameof(criteria));
        if (results == null) throw new ArgumentException("Results can not be null", nameof(results));

        Page = page;
        TotalPages = totalPages;
        Criteria = criteria;
        Results = results;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura <see cref="PaginatedQuery{T, C}"/> para la primera página.
    /// </summary>
    /// <param name="criteria">Criterios utilizados para la consulta.</param>
    /// <param name="results">Lista de resultados de la consulta.</param>
    /// <remarks>
    /// Este constructor establece automáticamente la página en 1 y el total de páginas en 1.
    /// </remarks>
    public PaginatedQuery(C criteria, List<T> results)
        : this(1, 1, criteria, results) { }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura <see cref="PaginatedQuery{T, C}"/> para la primera página con total de páginas específico.
    /// </summary>
    /// <param name="totalPages">Número total de páginas.</param>
    /// <param name="criteria">Criterios utilizados para la consulta.</param>
    /// <param name="results">Lista de resultados de la consulta.</param>
    /// <remarks>
    /// Este constructor establece automáticamente la página en 1.
    /// </remarks>
    public PaginatedQuery(ulong totalPages, C criteria, List<T> results)
        : this(1, totalPages, criteria, results) { }
}
