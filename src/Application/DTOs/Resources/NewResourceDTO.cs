using System.Text.Json.Nodes;

namespace Application.DTOs.Resources;

public sealed record NewResourceDTO
{
    public required string Title { get; init; }
    public required JsonNode Content { get; init; }
    public required ulong ProfessorId { get; init; }
}
