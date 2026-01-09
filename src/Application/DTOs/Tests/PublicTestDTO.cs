using Domain.Entities.PublicQuestions;

namespace Application.DTOs.Tests;

public sealed record PublicTestDTO
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
    /// Obtiene o establece el contenido del examen.
    /// </summary>
    /// <remarks>
    /// La estructura JSON puede contener preguntas, instrucciones y otros elementos del examen.
    /// </remarks>
    public required IEnumerable<IPublicQuestion> Content { get; set; }

    /// <summary>
    /// Obtiene o establece el límite de tiempo para completar el examen, en minutos.
    /// Un valor nulo indica que no hay límite de tiempo.
    /// </summary>
    public uint? TimeLimitMinutes { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador del profesor que creó el examen.
    /// </summary>
    public required ulong ProfessorId { get; set; }
}
