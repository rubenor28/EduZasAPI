using Application.DTOs.Tags;
using Domain.ValueObjects;

namespace Application.DTOs.Contacts;

public sealed class NewContactUseCaseDTO
{
    public required string Alias { get; set; }
    public Optional<string> Notes { get; set; } = Optional<string>.None();
    public required ulong AgendaOwnerId { get; set; }
    public required ulong ContactId { get; set; }

    public required IEnumerable<NewTagDTO> ContactTags { get; set; }
}
