using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Notifications;

/// <summary>
/// Mapeador de entidad EF a dominio para notificaciones.
/// </summary>
public class NotificationMapper : IMapper<Notification, NotificationDomain>
{
    /// <summary>
    /// Mapea una entidad de notificación de base de datos a un objeto de dominio.
    /// </summary>
    /// <param name="source">Entidad de base de datos.</param>
    /// <returns>Objeto de dominio de notificación.</returns>
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
