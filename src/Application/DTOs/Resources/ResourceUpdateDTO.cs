using System.Text.Json.Nodes;

namespace Application.DTOs.Resources;

public sealed record ResourceUpdateDTO
{
    public required Guid Id { get; init; }
    public required bool Active { get; init; }
    public required string Title { get; init; }
    public required JsonNode Content { get; init; }
    public required ulong ProfessorId { get; init; }
}
