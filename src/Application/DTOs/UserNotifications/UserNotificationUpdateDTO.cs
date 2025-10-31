using Domain.Entities;
using Domain.ValueObjects;

namespace Application.DTOs.UserNotifications;

public sealed record UserNotificationUpdateDTO : IIdentifiable<UserNotificationIdDTO>
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
