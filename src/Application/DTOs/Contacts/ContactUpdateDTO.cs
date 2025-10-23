using Domain.ValueObjects;

namespace Application.DTOs.Contacts;

public sealed class ContactUpdateDTO : IIdentifiable<ulong>
{
    public required ulong Id { get; set; }
    public required string Alias { get; set; }
    public Optional<string> Notes { get; set; } = Optional<string>.None();
    public required ulong AgendaOwnerId { get; set; }
    public required ulong ContactId { get; set; }
}
