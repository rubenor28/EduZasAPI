using Application.DTOs.UserNotifications;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.UserNotifications;

public class UserNotificationEFCreator(
    EduZasDotnetContext ctx,
    IMapper<NotificationPerUser, UserNotificationDomain> domainMapper,
    IMapper<NewUserNotificationDTO, NotificationPerUser> newEntityMapper
)
    : EFCreator<UserNotificationDomain, NewUserNotificationDTO, NotificationPerUser>(
        ctx,
        domainMapper,
        newEntityMapper
    );
