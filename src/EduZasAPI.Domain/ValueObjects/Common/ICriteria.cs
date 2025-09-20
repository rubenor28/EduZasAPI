namespace EduZasAPI.Domain.ValueObjects.Common;

/// <summary>
/// Define la interfaz para los criterios de consulta y paginación.
/// </summary>
public abstract class ICriteria
{
    /// <summary>
    /// Obtiene el número de página para la paginación de resultados.
    /// </summary>
    /// <value>Número de página actual (base 0 o 1 dependiendo de la implementación).</value>
    public ulong Page { get; }
}
