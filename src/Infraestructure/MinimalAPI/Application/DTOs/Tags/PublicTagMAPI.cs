namespace MinimalAPI.Application.DTOs.Tags;

public sealed record PublicTagMAPI
{
    public required ulong Id { get; set; }
    public required string Text { get; set; }
}
