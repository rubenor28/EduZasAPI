using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record TagDomain : IIdentifiable<string>
{
    public required string Text { get; set; }
    public required DateTime CreatedAt { get; set; }

    public string Id => Text;
}
