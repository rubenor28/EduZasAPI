using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tags;

public class TagMapper : IMapper<Tag, TagDomain>
{
    public TagDomain Map(Tag input) => new() { Text = input.Text, CreatedAt = input.CreatedAt };
}
