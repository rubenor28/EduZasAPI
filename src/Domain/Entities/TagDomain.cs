using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record TagDomain : IIdentifiable<ulong>
{
    public required ulong Id { get; set; }
    public required string Text { get; set; }
    public required DateTime CreatedAt { get; set; }
}
