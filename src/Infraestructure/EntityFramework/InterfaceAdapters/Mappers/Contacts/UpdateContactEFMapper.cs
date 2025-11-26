using Application.DTOs.Contacts;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Contacts;

public class UpdateContactEFMapper : IUpdateMapper<ContactUpdateDTO, AgendaContact>
{
    public void Map(ContactUpdateDTO source, AgendaContact destination)
    {
        destination.Alias = source.Alias;
        destination.Notes = source.Notes.ToNullable();
        destination.AgendaOwnerId = source.AgendaOwnerId;
        destination.UserId = source.UserId;
    }
}
