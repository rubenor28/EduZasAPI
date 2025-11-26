using Application.DTOs.UserNotifications;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.UserNotifications;

public class UpdateUserNotificationEFMapper
    : IUpdateMapper<UserNotificationUpdateDTO, NotificationPerUser>
{
    public void Map(UserNotificationUpdateDTO s, NotificationPerUser d)
    {
        d.UserId = s.UserId;
        d.NotificationId = s.NotificationId;
        d.Readed = s.Readed;
    }
}
