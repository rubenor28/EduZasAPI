using Application.DTOs.Common;
using Domain.Entities;

namespace Application.DTOs.ContactTags;

public sealed record ContactTagDTO
{
    public required ContactTagIdDTO Id { get; set; }
    public required Executor Executor { get; set; }
}
