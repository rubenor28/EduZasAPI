using Application.DTOs.ResourceViewSessions;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

public class NewResourceViewSessionMapper : IMapper<NewResourceViewSession, ResourceViewSession>
{
    public ResourceViewSession Map(NewResourceViewSession input) =>
        new()
        {
            ClassId = input.ClassId,
            UserId = input.UserId,
            ResourceId = input.ResourceId,
            StartTimeUtc = input.StartTimeUtc,
            EndTimeUtc = input.EndTimeUtc,
        };
}
