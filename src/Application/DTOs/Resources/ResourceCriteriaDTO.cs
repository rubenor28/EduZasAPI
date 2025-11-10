using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Resources;

public sealed record ResourceCriteriaDTO : CriteriaDTO
{
    public Optional<bool> Active { get; set; } = Optional<bool>.None();
    public Optional<StringQueryDTO> Title { get; set; } = Optional<StringQueryDTO>.None();
    public Optional<ulong> ProfessorId { get; set; } = Optional<ulong>.None();
    public Optional<string> ClassId { get; set; } = Optional<string>.None();
}
