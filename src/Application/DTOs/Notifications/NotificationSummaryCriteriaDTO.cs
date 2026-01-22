using Domain.ValueObjects;

namespace Application.DTOs.Notifications;

public sealed record NotificationSummaryCriteriaDTO : CriteriaDTO
{
    public required ulong UserId { get; init; }
    public bool? Readed { get; init; }
}
