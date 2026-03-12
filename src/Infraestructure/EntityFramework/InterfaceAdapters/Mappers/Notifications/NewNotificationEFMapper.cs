using Application.DTOs.Notifications;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Notifications;

/// <summary>
/// Mapeador de creación para notificaciones.
/// </summary>
public class NewNotificationEFMapper : IMapper<NewNotificationDTO, Notification>
{
    /// <summary>
    /// Mapea un DTO de nueva notificación a una entidad de base de datos.
    /// </summary>
    /// <param name="source">DTO de creación.</param>
    /// <returns>Entidad de base de datos.</returns>
    public Notification Map(NewNotificationDTO source) =>
        new()
        {
            Active = true,
            Title = s.Title,
            ClassId = s.ClassId,
        };
}
