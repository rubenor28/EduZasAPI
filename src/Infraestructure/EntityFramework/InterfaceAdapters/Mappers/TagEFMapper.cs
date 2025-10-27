using Application.DTOs.Tags;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public sealed class TagEFMapper : IMapper<Tag, TagDomain>, IMapper<NewTagDTO, Tag>
{
    public Tag Map(NewTagDTO input) => new() { Text = input.Text };

    public TagDomain Map(Tag input) =>
        new()
        {
            Id = input.TagId,
            Text = input.Text,
            CreatedAt = input.CreatedAt,
        };
}
