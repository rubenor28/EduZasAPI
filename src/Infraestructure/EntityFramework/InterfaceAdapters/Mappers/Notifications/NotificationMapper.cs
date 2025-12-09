using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Notifications;

/// <summary>
/// Mapeador de entidad EF a dominio para notificaciones.
/// </summary>
public class NotificationMapper : IMapper<Notification, NotificationDomain>
{
    /// <inheritdoc/>
    public NotificationDomain Map(Notification s) =>
        new()
        {
            Id = s.NotificationId,
            Title = s.Title,
            Active = s.Active ?? false,
            ClassId = s.ClassId,
            CreatedAt = s.CreatedAt,
        };
}
