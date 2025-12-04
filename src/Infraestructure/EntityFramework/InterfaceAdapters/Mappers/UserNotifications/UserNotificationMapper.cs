using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.UserNotifications;

public class UserNotificationMapper : IMapper<NotificationPerUser, UserNotificationDomain>
{
    public UserNotificationDomain Map(NotificationPerUser s) =>
        new()
        {
            Id = new() { NotificationId = s.NotificationId, UserId = s.UserId },
            Readed = s.Readed,
        };
}
