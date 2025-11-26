using Application.DTOs.Classes;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

public class NewClassEFMapper : IMapper<NewClassDTO, Class>
{
    public Class Map(NewClassDTO nc) =>
        new()
        {
            ClassId = nc.Id,
            ClassName = nc.ClassName,
            Color = nc.Color,
            Section = nc.Section.ToNullable(),
            Subject = nc.Subject.ToNullable(),
        };
}
