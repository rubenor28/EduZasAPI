namespace Application.DTOs.Notifications;

/// <summary>
/// Datos para crear una nueva notificación.
/// </summary>
public sealed record NewNotificationDTO
{
    /// <summary>Título de la notificación.</summary>
    public required string Title { get; init; }

    /// <summary>ID de la clase asociada.</summary>
    public required string ClassId { get; init; }
}
