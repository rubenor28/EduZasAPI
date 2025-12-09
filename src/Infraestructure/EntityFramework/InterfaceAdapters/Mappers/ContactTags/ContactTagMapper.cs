using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ContactTags;

/// <summary>
/// Mapeador de entidad EF a dominio para etiquetas de contacto.
/// </summary>
public class ContactTagMapper : IMapper<ContactTag, ContactTagDomain>
{
    /// <inheritdoc/>
    public ContactTagDomain Map(ContactTag input) => 
            new()
            {
                Id = new()
                {
                    Tag = input.TagText,
                    AgendaOwnerId = input.AgendaOwnerId,
                    UserId = input.UserId,
                },
                CreatedAt = input.CreatedAt,
            };
}
