namespace Application.DTOs.ResourceViewSessions;

/// <summary>
/// DTO para crear una nueva sesión de visualización de recurso.
/// Captura metadata de uso (tiempo, usuario, recurso) para reportes.
/// </summary>
public sealed record ResourceViewSession
{
    public required ulong UserId { get; init; }
    public required Guid ResourceId { get; init; }
    public required string ClassId { get; init; }
    public required DateTimeOffset StartTimeUtc { get; init; }
}
