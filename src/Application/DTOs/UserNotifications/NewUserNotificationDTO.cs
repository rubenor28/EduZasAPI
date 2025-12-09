namespace Application.DTOs.UserNotifications;

/// <summary>
/// Datos para asignar una notificación a un usuario.
/// </summary>
public sealed record NewUserNotificationDTO
{
    /// <summary>ID de la notificación.</summary>
    public required ulong NotificationId { get; init; }

    /// <summary>ID del usuario.</summary>
    public required ulong UserId { get; init; }
}
