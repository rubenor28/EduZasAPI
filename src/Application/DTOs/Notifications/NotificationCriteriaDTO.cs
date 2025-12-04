using Application.DTOs.Common;

namespace Application.DTOs.Notifications;

public sealed record NotificationCriteriaDTO : CriteriaDTO
{
    public string? ClassId { get; init; }
    public ulong? UserId { get; init; }
}
