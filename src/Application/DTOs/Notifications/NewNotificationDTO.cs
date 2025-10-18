namespace Application.DTOs.Notifications;

public sealed record NewNotificationDTO
{
    public required string Title { get; set; }
    public required string ClassId { get; set; }
}
