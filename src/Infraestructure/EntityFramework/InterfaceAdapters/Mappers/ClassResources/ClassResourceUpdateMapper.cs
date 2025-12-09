using Application.DTOs.ClassResources;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassResources;

public sealed class ClassResourceUpdateMapper : IUpdateMapper<ClassResourceDTO, ClassResource>
{
    public void Map(ClassResourceDTO s, ClassResource d)
    {
        d.Hidden = s.Hidden;
    }
}
