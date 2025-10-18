using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Notifications;

public sealed record NotificationCriteriaDTO : CriteriaDTO
{
    public Optional<string> ClassId { get; set; } = Optional<string>.None();
    public Optional<ulong> UserId { get; set; } = Optional<ulong>.None();
}
