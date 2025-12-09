using Application.DTOs.ClassResources;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassResources;

/// <summary>
/// Mapeador de actualización para recursos de clase.
/// </summary>
public sealed class ClassResourceUpdateMapper : IUpdateMapper<ClassResourceDTO, ClassResource>
{
    /// <inheritdoc/>
    public void Map(ClassResourceDTO s, ClassResource d)
    {
        d.Hidden = s.Hidden;
    }
}
