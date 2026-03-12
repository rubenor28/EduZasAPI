using Application.DTOs.UserNotifications;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.UserNotifications;

/// <summary>
/// Mapeador de creación para notificaciones de usuario.
/// </summary>
public class NewUserNotificationEFMapper : IMapper<NewUserNotificationDTO, NotificationPerUser>
{
    /// <summary>
    /// Mapea un DTO de nueva notificación de usuario a una entidad de base de datos.
    /// </summary>
    /// <param name="s">DTO de creación.</param>
    /// <returns>Entidad de base de datos.</returns>
    public NotificationPerUser Map(NewUserNotificationDTO s) =>
        new()
        {
            NotificationId = s.NotificationId,
            UserId = s.UserId,
            Readed = false,
        };
}
