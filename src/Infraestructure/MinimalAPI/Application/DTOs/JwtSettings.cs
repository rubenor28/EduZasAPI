namespace MinimalAPI.Application.DTOs;

/// <summary>
/// Configuración utilizada para la generación y validación de tokens JWT.
/// </summary>
public sealed record JwtSettings
{
    /// <summary>
    /// Clave secreta utilizada para firmar y validar los tokens JWT.
    /// </summary>
    /// <value>Cadena de texto que representa la clave secreta.</value>
    public required string Secret { get; init; }

    /// <summary>
    /// Tiempo de expiración de los tokens JWT en minutos.
    /// </summary>
    /// <value>Un entero que indica la duración en minutos antes de que el token expire.</value>
    public required int ExpiresMinutes { get; init; }
}
