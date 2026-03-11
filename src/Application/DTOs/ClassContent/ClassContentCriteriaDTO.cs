using Domain.ValueObjects;

namespace Application.DTOs.ClassContent;

/// <summary>
/// DTO que representa los criterios de búsqueda para el contenido de una clase (recursos y evaluaciones).
/// </summary>
public sealed record ClassContentCriteriaDTO : CriteriaDTO
{
    /// <summary>
    /// Obtiene o establece el ID de la clase a la que pertenece el contenido.
    /// </summary>
    public required string ClassId { get; set; }
    /// <summary>
    /// Obtiene o establece el filtro opcional por título del contenido.
    /// </summary>
    public StringQueryDTO? Title { get; set; }
    /// <summary>
    /// Obtiene o establece el filtro opcional por visibilidad del contenido.
    /// </summary>
    public bool? Visible { get; set; }
    /// <summary>
    /// Obtiene o establece el filtro opcional por tipo de contenido (TEST o RESOURCE).
    /// </summary>
    public ContentType? Type { get; set; }
}
