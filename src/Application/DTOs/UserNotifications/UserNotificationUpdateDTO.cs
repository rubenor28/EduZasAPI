namespace Application.DTOs.UserNotifications;

/// <summary>
/// Datos para actualizar el estado de una notificación de usuario.
/// </summary>
public sealed record UserNotificationUpdateDTO
{
    /// <summary>ID de la notificación.</summary>
    public required ulong NotificationId { get; init; }

    /// <summary>ID del usuario.</summary>
    public required ulong UserId { get; init; }

    /// <summary>Indica si la notificación fue leída.</summary>
    public required bool Readed { get; init; }
}
