using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Tests;

public sealed record TestCriteriaDTO : CriteriaDTO
{
    public Optional<StringQueryDTO> Title { get; set; } = Optional<StringQueryDTO>.None();
    public Optional<StringQueryDTO> Content { get; set; } = Optional<StringQueryDTO>.None();

    // TODO: Caso busqueda de Optional<Optional<uint>> para donde el tiempo del test no importe
    public Optional<uint> TimeLimitMinutes { get; set; } = Optional<uint>.None();
    public Optional<ulong> ProfessorId { get; set; } = Optional<ulong>.None();
}
