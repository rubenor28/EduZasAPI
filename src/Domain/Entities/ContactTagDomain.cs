using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record ContactTagIdDTO
{
    public required ulong TagId { get; set; }
    public required ulong AgendaContactId { get; set; }
}

public sealed record ContactTagDomain : IIdentifiable<ContactTagIdDTO>
{
    public required ContactTagIdDTO Id { get; set; }
    public required DateTime CreatedAt { get; set; }
}
