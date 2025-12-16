namespace MinimalAPI.Application.DTOs.RateLimit;

///<summary>
/// Configuración de rate limit global en la APP
///</summary>
public sealed record RateLimitSettings
{
    /// <summary>
    /// Rate limit para usuarios no autenticados
    /// </summary>
    public required RateLimitOptions Guest { get; init; }

    /// <summary>
    /// Rate limit para usuarios autenticados
    /// </summary>
    public required RateLimitOptions Authenticated { get; init; }
}
