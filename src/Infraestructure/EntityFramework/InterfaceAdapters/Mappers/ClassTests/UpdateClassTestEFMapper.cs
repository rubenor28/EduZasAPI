using Application.DTOs.ClassTests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

public class UpdateClassTestEFMapper : IUpdateMapper<ClassTestUpdateDTO, TestPerClass>
{
    public void Map(ClassTestUpdateDTO source, TestPerClass destination)
    {
        destination.Visible = source.Visible;
    }
}
