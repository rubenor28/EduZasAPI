using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record UserNotificationIdDTO
{
    public required ulong NotificationId { get; set; }
    public required ulong UserId { get; set; }
}

public sealed record UserNotificationDomain : IIdentifiable<UserNotificationIdDTO>
{
    /// <summary>
    /// Identificador únido de la notificación
    /// </summary>
    public required UserNotificationIdDTO Id { get; set; }

    /// <summary>
    /// Indicador de lectura de la notificación por parte del usuario
    /// </summary>
    public required bool Readed { get; set; }
}
