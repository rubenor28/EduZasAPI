namespace Application.DTOs.ResourceViewSessions;

/// <summary>
/// DTO para crear una nueva sesión de visualización de recurso.
/// Captura metadata de uso (tiempo, usuario, recurso) para reportes.
/// </summary>
public sealed record NewResourceViewSession
{
    /// <summary>
    /// Obtiene o establece el ID del usuario que visualiza el recurso.
    /// </summary>
    public required ulong UserId { get; set; }

    /// <summary>
    /// Obtiene o establece el ID del recurso que está siendo visualizado.
    /// </summary>
    public required Guid ResourceId { get; set; }

    /// <summary>
    /// Obtiene o establece el ID de la clase en el contexto de la cual se visualiza el recurso.
    /// </summary>
    public required string ClassId { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora (UTC) de inicio de la visualización.
    /// </summary>
    public required DateTimeOffset StartTimeUtc { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora (UTC) de finalización de la visualización. Es nulo si la sesión sigue activa.
    /// </summary>
    public required DateTimeOffset EndTimeUtc { get; set; }
}
