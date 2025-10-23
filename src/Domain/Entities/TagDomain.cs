namespace Domain.Entities;

public sealed record TagDomain
{
    public required ulong TagId { get; set; }
    public required string Text { get; set; }
    public required DateTime CreatedAt { get; set; }
}
