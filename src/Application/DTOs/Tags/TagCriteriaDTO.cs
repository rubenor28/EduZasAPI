using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Tags;

public sealed record TagCriteriaDTO : CriteriaDTO
{
    public Optional<StringQueryDTO> Text { get; set; } = Optional<StringQueryDTO>.None();
    public Optional<ulong> OwnerAgendaId { get; set; } = Optional<ulong>.None();
}
