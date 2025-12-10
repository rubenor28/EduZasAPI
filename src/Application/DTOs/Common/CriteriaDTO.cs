namespace Application.DTOs.Common;

/// <summary>
/// Define la interfaz para los criterios de consulta y paginación.
/// </summary>
public record CriteriaDTO
{
    /// <summary>
    /// Obtiene o establece el número de página actual (base 1).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Obtiene o establece la cantidad de elementos por página.
    /// </summary>
    public int PageSize { get; set; } = int.MaxValue;
}
