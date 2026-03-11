using Domain.Entities.QuestionAnswers;

namespace Domain.Entities;

/// <summary>
/// Representa los metadatos de una respuesta a un examen.
/// </summary>
public record AnswerMetadata
{
    /// <summary>
    /// Obtiene o inicializa un diccionario que almacena la calificación manual de preguntas específicas.
    /// La clave es el ID de la pregunta y el valor indica si se ha calificado manualmente.
    /// </summary>
    public IDictionary<Guid, bool> ManualGrade { get; init; } = new Dictionary<Guid, bool>();
}

/// <summary>
/// Representa la respuesta de un usuario a un examen en una clase específica.
/// </summary>
public class AnswerDomain
{
    /// <summary>
    /// Obtiene o establece el ID del usuario que envió la respuesta.
    /// </summary>
    public required ulong UserId { get; set; }

    /// <summary>
    /// Obtiene o establece el ID del examen al que corresponde la respuesta.
    /// </summary>
    public required Guid TestId { get; set; }

    /// <summary>
    /// Obtiene o establece el ID de la clase en la que se realizó el examen.
    /// </summary>
    public required string ClassId { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica si el intento de respuesta ha finalizado.
    /// </summary>
    public required bool TryFinished { get; set; }

    /// <summary>
    /// Obtiene o establece el contenido de la respuesta, con las respuestas a cada pregunta.
    /// </summary>
    public required IDictionary<Guid, IQuestionAnswer> Content { get; set; }

    /// <summary>
    /// Obtiene o establece los metadatos asociados a la respuesta.
    /// </summary>
    public required AnswerMetadata Metadata { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación de la respuesta.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la última modificación de la respuesta.
    /// </summary>
    public required DateTimeOffset ModifiedAt { get; set; }
}
