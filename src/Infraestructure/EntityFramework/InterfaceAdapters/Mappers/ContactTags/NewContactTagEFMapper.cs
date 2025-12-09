using Application.DTOs.ContactTags;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ContactTags;

/// <summary>
/// Mapeador de creación para etiquetas de contacto.
/// </summary>
public class NewContactTagEFMapper : IMapper<NewContactTagDTO, ContactTag>
{
    /// <inheritdoc/>
    public ContactTag Map(NewContactTagDTO input) =>
        new()
        {
            TagText = input.Tag,
            AgendaOwnerId = input.AgendaOwnerId,
            UserId = input.UserId,
        };
}
