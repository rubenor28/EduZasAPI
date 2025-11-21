using Application.DTOs.Common;
using Domain.Entities;

namespace Application.DTOs.ContactTags;

public sealed record DeleteContactTagDTO
{
    public required ContactTagIdDTO Id { get; set; }
    public required Executor Executor { get; set; }
}
