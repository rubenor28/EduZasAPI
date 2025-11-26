using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tags;

public class TagProjector : IEFProjector<Tag, TagDomain>
{
    public Expression<Func<Tag, TagDomain>> Projection =>
        input => new() { Text = input.Text, CreatedAt = input.CreatedAt };

    private static readonly Lazy<Func<Tag, TagDomain>> _mapFunc = new(() =>
        new TagProjector().Projection.Compile()
    );

    public TagDomain Map(Tag input) => _mapFunc.Value(input);
}
