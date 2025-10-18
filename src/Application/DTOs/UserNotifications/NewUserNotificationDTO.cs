namespace Application.DTOs.UserNotifications;

public sealed record NewUserNotificationDTO
{
    public required ulong NotificationId { get; set; }
    public required ulong UserId { get; set; }
}
