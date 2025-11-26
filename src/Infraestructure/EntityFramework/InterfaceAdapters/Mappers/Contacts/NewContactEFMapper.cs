using Application.DTOs.Contacts;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Contacts;

public class NewContactEFMapper : IMapper<NewContactDTO, AgendaContact>
{
    public AgendaContact Map(NewContactDTO input) =>
        new()
        {
            Alias = input.Alias,
            Notes = input.Notes.ToNullable(),
            UserId = input.UserId,
            AgendaOwnerId = input.AgendaOwnerId,
        };
}
