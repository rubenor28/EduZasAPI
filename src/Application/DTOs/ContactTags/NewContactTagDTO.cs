using Application.DTOs.Common;

namespace Application.DTOs.ContactTags;

public sealed record NewContactTagDTO
{
    public required string Tag { get; set; }
    public required ulong AgendaOwnerId { get; set; }
    public required ulong UserId { get; set; }
    public required Executor Executor { get; set; }
}
