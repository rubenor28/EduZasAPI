using System.Text.Json.Nodes;

namespace Application.DTOs.Resources;

/// <summary>
/// Datos para actualizar un recurso académico existente.
/// </summary>
public sealed record ResourceUpdateDTO
{
    /// <summary>
    /// Identificador del recurso a actualizar.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Nuevo estado de activación.
    /// </summary>
    public required bool Active { get; init; }

    /// <summary>
    /// Nuevo título del recurso.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Nuevo contenido del recurso.
    /// </summary>
    public required JsonNode Content { get; init; }

    /// <summary>
    /// Identificador del profesor (para validación).
    /// </summary>
    public required ulong ProfessorId { get; init; }
}
