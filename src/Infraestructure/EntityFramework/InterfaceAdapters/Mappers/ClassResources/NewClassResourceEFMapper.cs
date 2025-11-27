using Application.DTOs.ClassResources;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassResources;

public sealed class NewClassResourceEFMapper : IMapper<NewClassResourceDTO, ClassResource>
{
    public ClassResource Map(NewClassResourceDTO input) =>
        new() { ResourceId = input.ResourceId, ClassId = input.ClassId };
}
