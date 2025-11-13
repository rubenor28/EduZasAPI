namespace MinimalAPI.Application.DTOs.Resources;

public sealed record PublicResourceMAPI
{
    public required ulong Id { get; set; }
    public required bool Active { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required ulong ProfessorId { get; set; }
}
