using Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.Notifications;

public sealed record NotificationCriteriaMAPI : CriteriaDTO
{
    public string? ClassId { get; set; } = null;
    public ulong? UserId { get; set; } = null;
}
