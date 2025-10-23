using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class ContactDomain : IIdentifiable<ulong>
{
    public required ulong Id { get; set; }
    public required string Alias { get; set; }
    public required Optional<string> Notes { get; set; }
    public required ulong AgendaOwnerId { get; set; }
    public required ulong ContactId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ModifiedAt { get; set; }
}
