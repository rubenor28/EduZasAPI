namespace Application.DTOs.Notifications;

public sealed record NewNotificationDTO
{
    public required string Title { get; init; }
    public required string ClassId { get; init; }
}
