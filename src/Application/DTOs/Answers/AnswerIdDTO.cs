namespace Application.DTOs.Answers;

/// <summary>
/// DTO que representa el identificador compuesto de una respuesta.
/// </summary>
public sealed record AnswerIdDTO
{
    /// <summary>
    /// Obtiene o establece el ID del usuario asociado a la respuesta.
    /// </summary>
    public required ulong UserId { get; set; }
    /// <summary>
    /// Obtiene o establece el ID de la evaluación a la que pertenece la respuesta.
    /// </summary>
    public required Guid TestId { get; set; }
    /// <summary>
    /// Obtiene o establece el ID de la clase a la que está asociada la respuesta.
    /// </summary>
    public required string ClassId { get; set; }
}
