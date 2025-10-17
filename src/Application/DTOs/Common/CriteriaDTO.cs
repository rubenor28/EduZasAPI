namespace Application.DTOs.Common;

/// <summary>
/// Define la interfaz para los criterios de consulta y paginación.
/// </summary>
public abstract record CriteriaDTO
{
    /// <summary>
    /// Obtiene el número de página para la paginación de resultados.
    /// </summary>
    /// <value>Número de página actual (base 0 o 1 dependiendo de la implementación).</value>
    public int Page { get; set; } = 1;
}
