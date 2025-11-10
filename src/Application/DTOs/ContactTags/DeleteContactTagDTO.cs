using Application.DTOs.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.DTOs.ContactTags;

public sealed record DeleteContactTagDTO : IIdentifiable<ContactTagIdDTO>
{
    public required ContactTagIdDTO Id { get; set; }
    public required Executor Executor { get; set; }
}
