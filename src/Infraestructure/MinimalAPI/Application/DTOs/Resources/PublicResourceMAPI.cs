using System.Text.Json.Nodes;

namespace MinimalAPI.Application.DTOs.Resources;

public sealed record PublicResourceMAPI
{
    public required Guid Id { get; set; }
    public required bool Active { get; set; }
    public required string Title { get; set; }
    public required JsonNode Content { get; set; }
    public required ulong ProfessorId { get; set; }
}
