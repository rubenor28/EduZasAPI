using Application.DTOs.ClassTests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

public class UpdateClassTestEFMapper : IUpdateMapper<ClassTestDTO, TestPerClass>
{
    public void Map(ClassTestDTO source, TestPerClass destination)
    {
        destination.Visible = source.Visible;
    }
}