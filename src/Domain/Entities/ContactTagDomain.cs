using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record ContactTagIdDTO
{
    public required string Tag { get; set; }
    public required ulong AgendaOwnerId { get; set; }
    public required ulong ContactId { get; set; }
}

public sealed record ContactTagDomain : IIdentifiable<ContactTagIdDTO>
{
    public required ContactTagIdDTO Id { get; set; }
    public required DateTime CreatedAt { get; set; }
}
