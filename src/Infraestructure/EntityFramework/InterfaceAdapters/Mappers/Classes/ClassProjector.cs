using System.Linq.Expressions;
using Domain.Entities;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

public class ClassProjector : IEFProjector<Class, ClassDomain>
{
    public Expression<Func<Class, ClassDomain>> Projection =>
        ef =>
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

    private static readonly Lazy<Func<Class, ClassDomain>> _mapFunc = new(() =>
        new ClassProjector().Projection.Compile()
    );

    public ClassDomain Map(Class ef) => _mapFunc.Value(ef);
}
