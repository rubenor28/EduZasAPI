using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Contacts;

public sealed record ContactCriteriaDTO : CriteriaDTO
{
    public Optional<ulong> Id { get; set; } = Optional<ulong>.None();
    public  Optional<StringQueryDTO> Alias { get; set; } = Optional<StringQueryDTO>.None();
    public  Optional<string> Notes { get; set; } = Optional<string>.None();
}
