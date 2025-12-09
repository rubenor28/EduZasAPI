using Application.DTOs.ClassTests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

/// <summary>
/// Mapeador de actualización para exámenes de clase.
/// </summary>
public class UpdateClassTestEFMapper : IUpdateMapper<ClassTestDTO, TestPerClass>
{
    /// <inheritdoc/>
    public void Map(ClassTestDTO source, TestPerClass destination)
    {
        destination.Visible = source.Visible;
    }
}