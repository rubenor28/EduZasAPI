using Application.DTOs.Common;

namespace Application.DTOs.Notifications;

/// <summary>
/// Criterios de búsqueda para notificaciones.
/// </summary>
public sealed record NotificationCriteriaDTO : CriteriaDTO
{
    /// <summary>Filtra por ID de clase.</summary>
    public string? ClassId { get; init; }

    /// <summary>Filtra por ID de usuario.</summary>
    public ulong? UserId { get; init; }
}
