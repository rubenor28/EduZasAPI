using Application.DTOs.ClassResources;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassResources;

/// <summary>
/// Mapeador de creación para recursos de clase.
/// </summary>
public sealed class NewClassResourceEFMapper : IMapper<ClassResourceDTO, ClassResource>
{
    /// <inheritdoc/>
    public ClassResource Map(ClassResourceDTO input) =>
        new()
        {
            ResourceId = input.ResourceId,
            ClassId = input.ClassId,
            Hidden = input.Hidden,
        };
}
