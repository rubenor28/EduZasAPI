namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

/// <summary>
/// Representa una respuesta simple de la API que solo contiene un mensaje,
/// útil para operaciones que no requieren datos adicionales en el resultado.
/// </summary>
public readonly struct MessageResponse
{
    /// <summary>
    /// Mensaje que describe el resultado de la operación.
    /// </summary>
    public string Message { get; init; }
}
