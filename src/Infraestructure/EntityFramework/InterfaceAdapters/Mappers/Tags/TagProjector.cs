using System.Linq.Expressions;
using Application.DTOs.Tags;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tags;

public class TagProjector : IEFProjector<Tag, TagDomain, TagCriteriaDTO>
{
    public Expression<Func<Tag, TagDomain>> GetProjection(TagCriteriaDTO criteria) =>
        input => new() { Text = input.Text, CreatedAt = input.CreatedAt };
}
