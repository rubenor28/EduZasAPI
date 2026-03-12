using Application.DTOs.Classes;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

/// <summary>
/// Mapeador de creación para clases.
/// </summary>
public class NewClassEFMapper : IMapper<NewClassDTO, Class>
{
    /// <summary>
    /// Mapea un DTO de nueva clase a una entidad de base de datos.
    /// </summary>
    /// <param name="nc">DTO de creación.</param>
    /// <returns>Entidad de base de datos.</returns>
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
