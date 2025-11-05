using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Tests;

public sealed record TestUpdateDTO : IIdentifiable<ulong>
{
    public required ulong Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public Optional<uint> TimeLimitMinutes { get; set; } = Optional<uint>.None();
    public required ulong ProfessorId { get; set; }

    public required Executor Executor { get; set; }
}
