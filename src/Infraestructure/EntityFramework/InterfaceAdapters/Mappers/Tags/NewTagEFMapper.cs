using Application.DTOs.Tags;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tags;

/// <summary>
/// Mapeador de creación para etiquetas.
/// </summary>
public class NewTagEFMapper : IMapper<NewTagDTO, Tag>
{
    /// <inheritdoc/>
    public Tag Map(NewTagDTO input) => new() { Text = input.Text };
}
