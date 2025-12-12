using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tags;

/// <summary>
/// Mapeador de entidad EF a dominio para etiquetas.
/// </summary>
public class TagMapper : IMapper<Tag, TagDomain>
{
    /// <inheritdoc/>
    public TagDomain Map(Tag input) =>
        new()
        {
            Id = input.TagId,
            Text = input.Text,
            CreatedAt = input.CreatedAt,
        };
}
