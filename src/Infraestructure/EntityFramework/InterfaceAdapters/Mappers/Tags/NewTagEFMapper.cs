using Application.DTOs.Tags;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tags;

/// <summary>
/// Mapeador de creación para etiquetas.
/// </summary>
public class NewTagEFMapper : IMapper<NewTagDTO, Tag>
{
    /// <summary>
    /// Mapea un DTO de nueva etiqueta a una entidad de base de datos.
    /// </summary>
    /// <param name="input">DTO de creación.</param>
    /// <returns>Entidad de base de datos.</returns>
    public Tag Map(NewTagDTO input) => new() { Text = input.Text };
}
