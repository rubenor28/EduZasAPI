using Application.DTOs.Classes;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

public class UpdateClassEFMapper : IUpdateMapper<ClassUpdateDTO, Class>
{
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
