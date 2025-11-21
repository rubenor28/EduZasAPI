using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record ContactIdDTO
{
    public required ulong AgendaOwnerId { get; set; }
    public required ulong UserId { get; set; }
}

public sealed record ContactDomain
{
    public required ContactIdDTO Id { get; set; }
    public required string Alias { get; set; }
    public required Optional<string> Notes { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ModifiedAt { get; set; }
}
