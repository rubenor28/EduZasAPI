using Application.DTOs.ClassResources;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassResources;

public sealed class NewClassResourceEFMapper : IMapper<ClassResourceDTO, ClassResource>
{
    public ClassResource Map(ClassResourceDTO input) =>
        new()
        {
            ResourceId = input.ResourceId,
            ClassId = input.ClassId,
            Hidden = input.Hidden,
        };
}
