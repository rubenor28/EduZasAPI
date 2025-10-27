using Application.DTOs.Common;

namespace Application.DTOs.ContactTag;

public sealed record NewContactTagDTO
{
    public required ulong TagId { get; set; }
    public required ulong AgendaContactId { get; set; }
    public required Executor Executor { get; set; }
}
