namespace MinimalAPI.Application.DTOs.Resources;

public sealed class ResourceUpdateMAPI
{
    public required Guid Id { get; set; }
    public required bool Active { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required ulong ProfessorId { get; set; }
}
