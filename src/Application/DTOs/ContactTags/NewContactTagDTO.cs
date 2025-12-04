namespace Application.DTOs.ContactTags;

public sealed record NewContactTagDTO
{
    public required string Tag { get; init; }
    public required ulong AgendaOwnerId { get; init; }
    public required ulong UserId { get; init; }
}
