using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Contacts;

/// <summary>
/// Mapeador de entidad EF a dominio para contactos.
/// </summary>
public class ContactMapper : IMapper<AgendaContact, ContactDomain>
{
    /// <inheritdoc/>
    public ContactDomain Map(AgendaContact input) =>
        new()
        {
            Id = new() { AgendaOwnerId = input.AgendaOwnerId, UserId = input.UserId },
            Alias = input.Alias,
            Notes = input.Notes,
            CreatedAt = input.CreatedAt,
            ModifiedAt = input.ModifiedAt,
        };
}
