using Application.DTOs.UserNotifications;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class UserNotificationEFMapper
    : IMapper<NotificationPerUser, UserNotificationDomain>,
        IMapper<NewUserNotificationDTO, NotificationPerUser>,
        IUpdateMapper<UserNotificationUpdateDTO, NotificationPerUser>
{
    public NotificationPerUser Map(NewUserNotificationDTO s) =>
        new()
        {
            NotificationId = s.NotificationId,
            UserId = s.UserId,
            Readed = false,
        };

    public UserNotificationDomain Map(NotificationPerUser s) =>
        new()
        {
            Id = new() { NotificationId = s.NotificationId, UserId = s.UserId },
            Readed = s.Readed,
        };

    public void Map(UserNotificationUpdateDTO s, NotificationPerUser d)
    {
        d.UserId = s.UserId;
        d.NotificationId = s.NotificationId;
        d.Readed = s.Readed;
    }
}
