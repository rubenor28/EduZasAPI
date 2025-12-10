namespace Application.DTOs.Notifications;

public sealed record NotificationSummaryDTO(
    ulong Id,
    bool Readed,
    string Title,
    string ClassId,
    DateTime PublishDate
);
