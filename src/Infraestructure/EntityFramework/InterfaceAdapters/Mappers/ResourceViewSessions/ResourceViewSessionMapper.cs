using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

public class ResourceViewSessionMapper : IMapper<ResourceViewSession, ResourceViewSessionDomain>
{
    public ResourceViewSessionDomain Map(ResourceViewSession input) =>
        new()
        {
            Id = input.Id,
            UserId = input.UserId,
            ClassId = input.ClassId,
            ResourceId = input.ResourceId,
            StartTimeUtc = input.StartTimeUtc,
            EndTimeUtc = input.EndTimeUtc,
            CreatedAt = input.CreatedAt,
            ModifiedAt = input.ModifiedAt,
        };
}
