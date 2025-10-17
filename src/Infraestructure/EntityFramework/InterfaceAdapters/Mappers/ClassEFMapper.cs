using Application.DTOs.Classes;
using Domain.Entities;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class ClassEFMapper
    : IMapper<NewClassDTO, Class>,
        IMapper<Class, ClassDomain>,
        IUpdateMapper<ClassUpdateDTO, Class>
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

    public ClassDomain Map(Class ef) =>
        new()
        {
            Id = ef.ClassId,
            Active = ef.Active ?? false,
            ClassName = ef.ClassName,
            Color = ef.Color ?? "#007bff",
            Subject = ef.Subject.ToOptional(),
            Section = ef.Section.ToOptional(),
            CreatedAt = ef.CreatedAt,
            ModifiedAt = ef.ModifiedAt,
        };

    public void Map(ClassUpdateDTO cu, Class c)
    {
        c.ClassId = cu.Id;
        c.ClassName = cu.ClassName;
        c.Active = cu.Active;
        c.Color = cu.Color;
        c.Subject = cu.Subject.ToNullable();
        c.Section = cu.Section.ToNullable();
    }
}
