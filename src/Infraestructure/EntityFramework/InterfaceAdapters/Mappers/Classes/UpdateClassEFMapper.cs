using Application.DTOs.Classes;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

/// <summary>
/// Mapeador de actualización para clases.
/// </summary>
public class UpdateClassEFMapper : IUpdateMapper<ClassUpdateDTO, Class>
{
    /// <summary>
    /// Actualiza una entidad de clase con los datos del DTO.
    /// </summary>
    /// <param name="cu">DTO de actualización.</param>
    /// <param name="c">Entidad de base de datos.</param>
    public void Map(ClassUpdateDTO cu, Class c)
    {
        c.ClassId = cu.Id;
        c.ClassName = cu.ClassName;
        c.Active = cu.Active;
        c.Color = cu.Color;
        c.Subject = cu.Subject;
        c.Section = cu.Section;
    }
}
