using Application.DTOs.Notifications;
using InterfaceAdapters.Mappers.Common;

namespace MinimalAPI.Presentation.Mappers;

public sealed class UserNotificationCriteriaMAPIMapper
    : IMapper<int, ulong, NotificationCriteriaDTO>
{
    public NotificationCriteriaDTO Map(int page, ulong userId) =>
        new() { Page = page, UserId = userId };
}
