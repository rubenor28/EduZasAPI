using Application.DTOs.UserNotifications;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.UserNotifications;

/// <summary>
/// Mapeador de actualización para notificaciones de usuario.
/// </summary>
public class UpdateUserNotificationEFMapper
    : IUpdateMapper<UserNotificationUpdateDTO, NotificationPerUser>
{
    /// <inheritdoc/>
    public void Map(UserNotificationUpdateDTO s, NotificationPerUser d)
    {
        d.UserId = s.UserId;
        d.NotificationId = s.NotificationId;
        d.Readed = s.Readed;
    }
}
