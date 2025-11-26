using Application.DTOs.UserNotifications;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.UserNotifications;

public class NewUserNotificationEFMapper : IMapper<NewUserNotificationDTO, NotificationPerUser>
{
    public NotificationPerUser Map(NewUserNotificationDTO s) =>
        new()
        {
            NotificationId = s.NotificationId,
            UserId = s.UserId,
            Readed = false,
        };
}
