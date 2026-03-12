using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.UserNotifications;

/// <summary>
/// Mapeador de entidad EF a dominio para notificaciones de usuario.
/// </summary>
public class UserNotificationMapper : IMapper<NotificationPerUser, UserNotificationDomain>
{
    /// <summary>
    /// Mapea una entidad de notificación por usuario a un objeto de dominio.
    /// </summary>
    /// <param name="s">Entidad de base de datos.</param>
    /// <returns>Objeto de dominio.</returns>
    public UserNotificationDomain Map(NotificationPerUser s) =>
        new()
        {
            Id = new() { NotificationId = s.NotificationId, UserId = s.UserId },
            Readed = s.Readed,
        };
}
