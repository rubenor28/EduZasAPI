namespace MinimalAPI.Application.DTOs.Resources;

public sealed record NewResourceMAPI
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required ulong ProfessorId { get; set; }
}
