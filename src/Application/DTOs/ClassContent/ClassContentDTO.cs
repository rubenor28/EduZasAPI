namespace Application.DTOs.ClassContent;

/// <summary>
/// Define los tipos de contenido que pueden asociarse a una clase.
/// </summary>
public enum ContentType
{
    /// <summary>
    /// Indica que el contenido es una evaluación (examen).
    /// </summary>
    TEST,
    /// <summary>
    /// Indica que el contenido es un recurso (material de estudio, nota).
    /// </summary>
    RESOURCE,
}

/// <summary>
/// DTO que representa un elemento de contenido asociado a una clase, como un recurso o una evaluación.
/// </summary>
public sealed class ClassContentDTO
{
    /// <summary>
    /// Obtiene o establece el identificador único del contenido.
    /// </summary>
    public required Guid Id { get; set; }
    /// <summary>
    /// Obtiene o establece el título del contenido.
    /// </summary>
    public required string Title { get; set; }
    /// <summary>
    /// Obtiene o establece el tipo de contenido (TEST o RESOURCE).
    /// </summary>
    public required ContentType Type { get; set; }
    /// <summary>
    /// Obtiene o establece un valor opcional que indica si el contenido está oculto.
    /// </summary>
    public bool? Hidden { get; set; } = null;
    /// <summary>
    /// Obtiene o establece la fecha de publicación del contenido.
    /// </summary>
    public required DateTimeOffset PublishDate { get; set; }
}
