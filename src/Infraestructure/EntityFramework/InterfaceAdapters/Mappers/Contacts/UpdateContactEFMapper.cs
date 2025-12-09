using Application.DTOs.Contacts;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Contacts;

/// <summary>
/// Mapeador de actualización para contactos.
/// </summary>
public class UpdateContactEFMapper : IUpdateMapper<ContactUpdateDTO, AgendaContact>
{
    /// <inheritdoc/>
    public void Map(ContactUpdateDTO source, AgendaContact destination)
    {
        destination.Alias = source.Alias;
        destination.Notes = source.Notes;
        destination.AgendaOwnerId = source.AgendaOwnerId;
        destination.UserId = source.UserId;
    }
}
