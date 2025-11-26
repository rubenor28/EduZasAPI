using Application.DTOs.ContactTags;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ContactTags;

public class NewContactTagEFMapper : IMapper<NewContactTagDTO, ContactTag>
{
    public ContactTag Map(NewContactTagDTO input) =>
        new()
        {
            TagText = input.Tag,
            AgendaOwnerId = input.AgendaOwnerId,
            UserId = input.UserId,
        };
}
