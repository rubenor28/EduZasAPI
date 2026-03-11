using Domain.ValueObjects;

namespace Application.DTOs.Answers;

/// <summary>
/// DTO que representa los criterios de búsqueda para filtrar respuestas a evaluaciones.
/// </summary>
public record AnswerCriteriaDTO : CriteriaDTO
{
    /// <summary>
    /// Obtiene o establece el ID de la evaluación para filtrar las respuestas.
    /// </summary>
    public Guid? TestId { get; set; }
    /// <summary>
    /// Obtiene o establece el ID del usuario para filtrar las respuestas.
    /// </summary>
    public ulong? UserId { get; set; }
    /// <summary>
    /// Obtiene o establece el ID de la clase para filtrar las respuestas.
    /// </summary>
    public string? ClassId { get; set; }
    /// <summary>
    /// Obtiene o establece el ID del propietario de la evaluación para filtrar las respuestas.
    /// </summary>
    public ulong? TestOwnerId { get; set; }
}
