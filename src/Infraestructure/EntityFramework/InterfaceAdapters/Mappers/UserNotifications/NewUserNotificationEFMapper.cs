using Application.DTOs.UserNotifications;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.UserNotifications;

/// <summary>
/// Mapeador de creación para notificaciones de usuario.
/// </summary>
public class NewUserNotificationEFMapper : IMapper<NewUserNotificationDTO, NotificationPerUser>
{
    /// <inheritdoc/>
    public NotificationPerUser Map(NewUserNotificationDTO s) =>
        new()
        {
            NotificationId = s.NotificationId,
            UserId = s.UserId,
            Readed = false,
        };
}
