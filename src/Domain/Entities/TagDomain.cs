namespace Domain.Entities;

public sealed record TagDomain
{
    public required string Text { get; set; }
    public required DateTime CreatedAt { get; set; }

    public string Id => Text;
}
