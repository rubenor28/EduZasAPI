using Domain.Entities.Questions;

namespace Domain.Entities;

/// <summary>
/// Representa un examen o evaluación en el sistema.
/// </summary>
/// <remarks>
/// Un examen contiene una estructura de contenido flexible en formato JSON,
/// un título, y metadatos como el límite de tiempo y el profesor que lo creó.
/// </remarks>
public sealed record TestDomain
{
    /// <summary>
    /// Obtiene o establece el identificador único del examen (GUID).
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Obtiene o establece si el examen está activo y disponible para ser asignado.
    /// </summary>
    public required bool Active { get; set; }

    /// <summary>
    /// Obtiene o establece el título del examen.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Obtiene o establece el color de la carta del examen.
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// Obtiene o establece el contenido del examen, representado como un nodo JSON.
    /// </summary>
    /// <remarks>
    /// La estructura JSON puede contener preguntas, instrucciones y otros elementos del examen.
    /// </remarks>
    public required IDictionary<Guid, IQuestion> Content { get; set; }

    /// <summary>
    /// Obtiene o establece el límite de tiempo para completar el examen, en minutos.
    /// Un valor nulo indica que no hay límite de tiempo.
    /// </summary>
    public uint? TimeLimitMinutes { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador del profesor que creó el examen.
    /// </summary>
    public required ulong ProfessorId { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación del examen.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la última modificación del examen.
    /// </summary>
    public required DateTimeOffset ModifiedAt { get; set; }

    /// <summary>
    /// Obtiene si el test tiene preguntas que requieren intervencion manual para calificar.
    /// </summary>
    public IEnumerable<Guid> RequiredManualGrade =>
        Content.Where((pair) => pair.Value.RequiresManualGrade).Select(pair => pair.Key);
}
