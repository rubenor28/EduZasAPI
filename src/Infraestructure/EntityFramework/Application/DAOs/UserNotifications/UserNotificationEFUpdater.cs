using Application.DAOs;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.UserNotifications;

public class UserNotificationEFUpdater
    : CompositeKeyEFUpdater<UserNotificationIdDTO, UserNotificationDomain, NotificationPerUser>
{
    public UserNotificationEFUpdater(
        EduZasDotnetContext ctx,
        IMapper<NotificationPerUser, UserNotificationDomain> domainMapper,
        IUpdaterAsync<UserNotificationDomain, NotificationPerUser> updateMapper
    )
        : base(ctx, domainMapper, updateMapper) { }
}
