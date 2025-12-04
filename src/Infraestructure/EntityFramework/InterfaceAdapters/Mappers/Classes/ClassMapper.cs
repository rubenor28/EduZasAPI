using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

public class ClassMapper : IMapper<Class, ClassDomain>
{
    public ClassDomain Map(Class ef) =>
        new()
        {
            Id = ef.ClassId,
            Active = ef.Active ?? false,
            ClassName = ef.ClassName,
            Color = ef.Color ?? "#007bff",
            Subject = ef.Subject,
            Section = ef.Section,
            CreatedAt = ef.CreatedAt,
            ModifiedAt = ef.ModifiedAt,
        };
}
