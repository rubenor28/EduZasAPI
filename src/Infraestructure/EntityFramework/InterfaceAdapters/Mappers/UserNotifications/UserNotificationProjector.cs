using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.UserNotifications;

public class UserNotificationProjector : IEFProjector<NotificationPerUser, UserNotificationDomain>
{
    public Expression<Func<NotificationPerUser, UserNotificationDomain>> Projection =>
        s =>
            new()
            {
                Id = new() { NotificationId = s.NotificationId, UserId = s.UserId },
                Readed = s.Readed,
            };

    private static readonly Lazy<Func<NotificationPerUser, UserNotificationDomain>> _mapFunc = new(
        () =>
            new UserNotificationProjector().Projection.Compile()
    );

    public UserNotificationDomain Map(NotificationPerUser s) => _mapFunc.Value(s);
}
