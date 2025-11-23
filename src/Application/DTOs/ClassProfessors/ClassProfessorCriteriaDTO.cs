using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.ClassProfessors;

public sealed record ClassProfessorCriteriaDTO : CriteriaDTO
{
    public Optional<ulong> UserId { get; set; } = Optional<ulong>.None();
    public Optional<string> ClassId { get; set; } = Optional<string>.None();
    public Optional<bool> IsOwner { get; set; } = Optional<bool>.None();
}
