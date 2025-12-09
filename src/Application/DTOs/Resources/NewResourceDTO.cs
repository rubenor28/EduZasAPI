using System.Text.Json.Nodes;

namespace Application.DTOs.Resources;

/// <summary>
/// Datos requeridos para crear un nuevo recurso académico.
/// </summary>
public sealed record NewResourceDTO
{
    /// <summary>
    /// Título del recurso.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Color de la tarjeta del recurso.
    /// </summary>
    public required string Color { get; init; }

    /// <summary>
    /// Contenido del recurso en formato JSON (ej. documento enriquecido).
    /// </summary>
    public required JsonNode Content { get; init; }

    /// <summary>
    /// Identificador del profesor creador.
    /// </summary>
    public required ulong ProfessorId { get; init; }
}
