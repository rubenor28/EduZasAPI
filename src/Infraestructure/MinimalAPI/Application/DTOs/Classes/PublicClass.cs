namespace MinimalAPI.Application.DTOs.Classes;

public sealed record PublicClassMAPI
{
    public required string Id { get; set; }
    public required bool Active { get; set; }
    public required string ClassName { get; set; }
    public string? Subject { get; set; }
    public string? Section { get; set; }
    public required string Color { get; set; }
}
