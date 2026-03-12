using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ContactTags;

/// <summary>
/// Mapeador de entidad EF a dominio para etiquetas de contacto.
/// </summary>
public class ContactTagMapper : IMapper<ContactTag, ContactTagDomain>
{
    /// <summary>
    /// Mapea una entidad de etiqueta de contacto a un objeto de dominio.
    /// </summary>
    /// <param name="input">Entidad de base de datos.</param>
    /// <returns>Objeto de dominio.</returns>
    public ContactTagDomain Map(ContactTag input) =>
        new()
        {
            Id = new()
            {
                TagId = input.TagId,
                AgendaOwnerId = input.AgendaOwnerId,
                UserId = input.UserId,
            },
            CreatedAt = input.CreatedAt,
        };
}
