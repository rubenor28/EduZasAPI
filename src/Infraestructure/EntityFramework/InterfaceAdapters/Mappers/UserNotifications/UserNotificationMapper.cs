using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.UserNotifications;

/// <summary>
/// Mapeador de entidad EF a dominio para notificaciones de usuario.
/// </summary>
public class UserNotificationMapper : IMapper<NotificationPerUser, UserNotificationDomain>
{
    /// <inheritdoc/>
    public UserNotificationDomain Map(NotificationPerUser s) =>
        new()
        {
            Id = new() { NotificationId = s.NotificationId, UserId = s.UserId },
            Readed = s.Readed,
        };
}
