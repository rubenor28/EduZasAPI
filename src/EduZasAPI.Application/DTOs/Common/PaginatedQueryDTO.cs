namespace EduZasAPI.Application.DTOs.Common;

/// <summary>
/// Representa el resultado de una consulta paginada con criterios específicos.
/// </summary>
/// <typeparam name="T">Tipo de los elementos en los resultados. Debe ser un tipo no nulo.</typeparam>
/// <typeparam name="C">Tipo de los criterios de consulta. Debe implementar <see cref="ICriteria"/> y ser no nulo.</typeparam>
/// <remarks>
/// Esta clase encapsula los resultados de una consulta paginada junto con la información
/// de paginación y los criterios utilizados para la búsqueda. Los campos principales
/// son obligatorios para garantizar la integridad del resultado.
/// </remarks>
public class PaginatedQuery<T, C>
where T : notnull
where C : notnull, ICriteriaDTO
{
    /// <summary>
    /// Obtiene los criterios utilizados para realizar la consulta.
    /// </summary>
    /// <value>Instancia de los criterios de búsqueda. Campo obligatorio.</value>
    public required C Criteria { get; init; }

    /// <summary>
    /// Obtiene la lista de resultados de la consulta paginada.
    /// </summary>
    /// <value>Lista de elementos resultantes de la consulta. Campo obligatorio.</value>
    public required List<T> Results { get; init; }

    /// <summary>
    /// Obtiene el número de página actual de los resultados.
    /// </summary>
    /// <value>
    /// Número de página actual (comienza en 1). 
    /// Valor por defecto: 1.
    /// </value>
    public ulong Page { get; init; } = 1;

    /// <summary>
    /// Obtiene el número total de páginas disponibles para la consulta.
    /// </summary>
    /// <value>
    /// Total de páginas en la consulta.
    /// Valor por defecto: 1.
    /// </value>
    public ulong TotalPages { get; init; } = 1;
}
