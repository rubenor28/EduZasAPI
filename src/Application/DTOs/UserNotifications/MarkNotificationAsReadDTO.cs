namespace Application.DTOs.UserNotifications;

public sealed record MarkNotificationAsReadDTO
{
    public required ulong NotificationId { get; set; }
    public required ulong UserId { get; set; }
}
