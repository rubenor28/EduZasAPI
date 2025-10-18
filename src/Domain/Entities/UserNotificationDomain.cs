using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record UserNotificationIdDTO
{
    public required ulong NotificationId { get; set; }
    public required ulong UserId { get; set; }
}

public sealed record UserNotificationDomain : IIdentifiable<UserNotificationIdDTO>
{
    public required UserNotificationIdDTO Id { get; set; }
    public required bool Readed { get; set; }
}
