using Application.DTOs.Notifications;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class NotificationEFMapper
    : IMapper<Notification, NotificationDomain>,
        IMapper<NewNotificationDTO, Notification>
{
    public Notification Map(NewNotificationDTO s) =>
        new()
        {
            Active = true,
            Title = s.Title,
            ClassId = s.ClassId,
        };

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
