namespace Application.DTOs.Common;

/// <summary>
/// Define la interfaz para los criterios de consulta y paginación.
/// </summary>
public abstract record CriteriaDTO
{
    /// <summary>
    /// Obtiene el número de página para la paginación de resultados.
    /// </summary>
    /// <remarks>El valor por defecto es 1</remarks>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Número de elementos que puede tener una pagina.
    /// </summary>
    /// <value>Número de página actual.</value>
    /// <remarks>El valor por defecto es int.MaxValue</remarks>
    public int PageSize { get; set; } = int.MaxValue;
}
