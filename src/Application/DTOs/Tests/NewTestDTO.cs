using System.Text.Json.Nodes;

namespace Application.DTOs.Tests;

public sealed record NewTestDTO
{
    public required string Title { get; init; }
    public required JsonNode Content { get; init; }
    public uint? TimeLimitMinutes { get; init; } 
    public required ulong ProfessorId { get; init; }
}
