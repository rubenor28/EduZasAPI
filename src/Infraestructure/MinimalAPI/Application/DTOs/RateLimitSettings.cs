namespace MinimalAPI.Application.DTOs;

/// <summary>
/// Representa la configuración para la limitación de tasas (Rate Limiting).
/// </summary>
public sealed record RateLimitSettings
{
    /// <summary>
    /// Obtiene el número máximo de permisos (solicitudes) permitidos por ventana de tiempo.
    /// </summary>
    public int PermitLimit { get; init; }
    /// <summary>
    /// Obtiene la duración de la ventana de tiempo en segundos para la limitación de tasas.
    /// </summary>
    public int WindowSeconds { get; init; }
    /// <summary>
    /// Obtiene el límite de elementos que pueden ser encolados cuando el límite de permisos es excedido.
    /// </summary>
    public int QueueLimit { get; init; }
}
