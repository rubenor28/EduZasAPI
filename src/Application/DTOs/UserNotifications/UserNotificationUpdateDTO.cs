namespace Application.DTOs.UserNotifications;

public sealed record UserNotificationUpdateDTO
{
    /// <summary>
    /// Identificador únido de la notificación
    /// </summary>
    public required ulong NotificationId { get; set; }

    /// <summary>
    /// Identificador únido del usuario
    /// </summary>
    public required ulong UserId { get; set; }

    /// <summary>
    /// Indicador de lectura de la notificación por parte del usuario
    /// </summary>
    public required bool Readed { get; set; }
}
