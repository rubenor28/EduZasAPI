using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record TestDomain
{
    public required ulong Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public Optional<uint> TimeLimitMinutes { get; set; } = Optional<uint>.None();
    public required ulong ProfessorId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ModifiedAt { get; set; }
}
