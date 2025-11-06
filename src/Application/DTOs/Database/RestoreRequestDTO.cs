using Application.DTOs.Common;

namespace Application.DTOs.Database;

/// <summary>
/// DTO para solicitar la restauración desde un respaldo.
/// </summary>
public sealed class RestoreRequestDTO
{
    /// <summary>
    /// Entidad que ejecuta la acción.
    /// </summary>
    public required Executor Executor { get; init; }

    /// <summary>
    /// Stream de datos con el contenido del respaldo.
    /// </summary>
    public required Stream InputStream { get; init; }
}

