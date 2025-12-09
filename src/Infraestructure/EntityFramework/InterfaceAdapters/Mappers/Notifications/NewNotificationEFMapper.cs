using Application.DTOs.Notifications;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Notifications;

/// <summary>
/// Mapeador de creación para notificaciones.
/// </summary>
public class NewNotificationEFMapper : IMapper<NewNotificationDTO, Notification>
{
    /// <inheritdoc/>
    public Notification Map(NewNotificationDTO s) =>
        new()
        {
            Active = true,
            Title = s.Title,
            ClassId = s.ClassId,
        };
}
