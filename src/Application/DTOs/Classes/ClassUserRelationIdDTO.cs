namespace Application.DTOs.Classes;

/// <summary>
/// Representa el identificador compuesto para relaciones entre usuarios y clases.
/// </summary>
public sealed record ClassUserRelationIdDTO
{
    /// <summary>
    /// Obtiene o establece el identificador del usuario.
    /// </summary>
    public required ulong UserId { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador de la clase.
    /// </summary>
    public required string ClassId { get; set; }
}
