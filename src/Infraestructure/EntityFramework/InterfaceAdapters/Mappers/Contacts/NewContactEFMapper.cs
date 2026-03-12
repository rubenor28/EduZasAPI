using Application.DTOs.Contacts;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Contacts;

/// <summary>
/// Mapeador de creación para contactos.
/// </summary>
public class NewContactEFMapper : IMapper<NewContactDTO, AgendaContact>
{
    /// <summary>
    /// Mapea un DTO de nuevo contacto a una entidad de base de datos.
    /// </summary>
    /// <param name="input">DTO de creación.</param>
    /// <returns>Entidad de base de datos.</returns>
    public AgendaContact Map(NewContactDTO input) =>
        new()
        {
            Alias = input.Alias,
            Notes = input.Notes,
            UserId = input.UserId,
            AgendaOwnerId = input.AgendaOwnerId,
        };
}
