using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.UserNotifications;

public class UserNotificationEFReader(
    EduZasDotnetContext ctx,
    IEFProjector<NotificationPerUser, UserNotificationDomain> projector
) : EFReader<UserNotificationIdDTO, UserNotificationDomain, NotificationPerUser>(ctx, projector)
{
    protected override Expression<Func<NotificationPerUser, bool>> GetIdPredicate(
        UserNotificationIdDTO id
    ) => n => n.UserId == id.UserId && n.NotificationId == id.NotificationId;
}
