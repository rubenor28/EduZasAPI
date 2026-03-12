using Application.DTOs.ContactTags;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ContactTags;

/// <summary>
/// Mapeador de creación para etiquetas de contacto.
/// </summary>
public class NewContactTagEFMapper : IMapper<NewContactTagDTO, ContactTag>
{
    /// <summary>
    /// Mapea un DTO de nueva etiqueta de contacto a una entidad de base de datos.
    /// </summary>
    /// <param name="input">DTO de creación.</param>
    /// <returns>Entidad de base de datos.</returns>
    public ContactTag Map(NewContactTagDTO input) =>
        new()
        {
            TagId = input.TagId,
            AgendaOwnerId = input.AgendaOwnerId,
            UserId = input.UserId,
        };
}
