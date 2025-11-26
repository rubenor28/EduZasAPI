using Application.DTOs.Tags;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tags;

public class NewTagEFMapper : IMapper<NewTagDTO, Tag>
{
    public Tag Map(NewTagDTO input) => new() { Text = input.Text };
}
