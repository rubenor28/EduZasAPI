using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassResources;

public class ClassResourceProjector : IEFProjector<ClassResource, ClassResourceDomain>
{
    public Expression<Func<ClassResource, ClassResourceDomain>> Projection =>
        input =>
            new()
            {
                ClassId = input.ClassId,
                ResourceId = input.ResourceId,
                Hidden = input.Hidden,
                CreatedAt = input.CreatedAt,
            };

    private static readonly Lazy<Func<ClassResource, ClassResourceDomain>> _mapFunc = new(() =>
        new ClassResourceProjector().Projection.Compile());

    public ClassResourceDomain Map(ClassResource input)
      => _mapFunc.Value(input);
}
