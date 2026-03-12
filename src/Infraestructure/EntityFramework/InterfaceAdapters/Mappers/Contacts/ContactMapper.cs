using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Contacts;

/// <summary>
/// Mapeador de entidad EF a dominio para contactos.
/// </summary>
public class ContactMapper : IMapper<AgendaContact, ContactDomain>
{
    /// <summary>
    /// Mapea una entidad de contacto de base de datos a un objeto de dominio.
    /// </summary>
    /// <param name="input">Entidad de base de datos.</param>
    /// <returns>Objeto de dominio de contacto.</returns>
    public ContactDomain Map(AgendaContact input) =>
        new()
        {
            AgendaOwnerId = input.AgendaOwnerId,
            UserId = input.UserId,
            Alias = input.Alias,
            Notes = input.Notes,
            CreatedAt = input.CreatedAt,
            ModifiedAt = input.ModifiedAt,
        };
}
