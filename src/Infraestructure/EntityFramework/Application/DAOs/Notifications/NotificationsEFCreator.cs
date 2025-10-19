using Application.DTOs.Notifications;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Notifications;

public class NotificationEFCreator(
    EduZasDotnetContext ctx,
    IMapper<Notification, NotificationDomain> domainMapper,
    IMapper<NewNotificationDTO, Notification> newEntityMapper
)
    : EFCreator<NotificationDomain, NewNotificationDTO, Notification>(
        ctx,
        domainMapper,
        newEntityMapper
    );
