namespace MinimalAPI.Application.DTOs;

/// <summary>
/// Representa una respuesta simple de la API que solo contiene un mensaje,
/// útil para operaciones que no requieren datos adicionales en el resultado.
/// </summary>
public sealed record MessageResponse
{
    /// <summary>
    /// Mensaje que describe el resultado de la operación.
    /// </summary>
    public required string Message { get; init; }
}
