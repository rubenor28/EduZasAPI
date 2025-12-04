namespace Application.DTOs.UserNotifications;

public sealed record NewUserNotificationDTO
{
    public required ulong NotificationId { get; init; }
    public required ulong UserId { get; init; }
}
