using Application.DTOs.Notifications;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Notifications;

public class NotificationEFCreator : EFCreator<NotificationDomain, NewNotificationDTO, Notification>
{
    public NotificationEFCreator(
        EduZasDotnetContext ctx,
        IMapper<Notification, NotificationDomain> domainMapper,
        IMapper<NewNotificationDTO, Notification> newEntityMapper
    )
        : base(ctx, domainMapper, newEntityMapper) { }
}
