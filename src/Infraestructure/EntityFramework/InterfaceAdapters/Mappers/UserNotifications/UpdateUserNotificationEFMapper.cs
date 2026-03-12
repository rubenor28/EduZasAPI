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
    /// <summary>
    /// Actualiza una entidad de notificación de usuario con los datos del DTO.
    /// </summary>
    /// <param name="s">DTO de actualización.</param>
    /// <param name="d">Entidad de base de datos.</param>
    public void Map(UserNotificationUpdateDTO s, NotificationPerUser d)
    {
        d.UserId = s.UserId;
        d.NotificationId = s.NotificationId;
        d.Readed = s.Readed;
    }
}
