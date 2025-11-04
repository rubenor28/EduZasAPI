using Application.DTOs.Common;
using Domain.Entities;

namespace Application.DTOs.ContactTags;

public sealed record NewContactTagDTO
{
    public required string Tag { get; set; }
    public required ContactIdDTO ContactId { get; set; }
    public required Executor Executor { get; set; }
}
