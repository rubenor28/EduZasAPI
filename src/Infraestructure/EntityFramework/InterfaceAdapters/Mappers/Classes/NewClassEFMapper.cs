using Application.DTOs.Classes;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

/// <summary>
/// Mapeador de creación para clases.
/// </summary>
public class NewClassEFMapper : IMapper<NewClassDTO, Class>
{
    /// <inheritdoc/>
    public Class Map(NewClassDTO nc) =>
        new()
        {
            ClassId = nc.Id,
            ClassName = nc.ClassName,
            Color = nc.Color,
            Section = nc.Section,
            Subject = nc.Subject,
        };
}
