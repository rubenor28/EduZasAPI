using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record NotificationDomain : IIdentifiable<ulong>
{
    public required ulong Id { get; set; }

    public required bool Active { get; set; }

    public required string Title { get; set; }

    public required string ClassId { get; set; }

    public required DateTime CreatedAt { get; set; }
}
