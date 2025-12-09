using Application.DTOs.Common;

namespace Application.DTOs.Tests;

public sealed record TestCriteriaDTO : CriteriaDTO
{
    public StringQueryDTO? Title { get; init; }
    public bool? Active { get; init; }

    // TODO: Caso busqueda de Optional<uint?> para donde el tiempo del test no importe
    public uint? TimeLimitMinutes { get; init; }
    public ulong? ProfessorId { get; init; }
    public string? AssignedInClass { get; init; }
}
