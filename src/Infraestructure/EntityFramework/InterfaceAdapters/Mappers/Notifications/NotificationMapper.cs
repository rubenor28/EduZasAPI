using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Notifications;

public class NotificationMapper : IMapper<Notification, NotificationDomain>
{
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
