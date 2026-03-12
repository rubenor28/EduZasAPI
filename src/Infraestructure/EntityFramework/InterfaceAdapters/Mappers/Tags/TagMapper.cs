using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tags;

/// <summary>
/// Mapeador de entidad EF a dominio para etiquetas.
/// </summary>
public class TagMapper : IMapper<Tag, TagDomain>
{
    /// <summary>
    /// Mapea una entidad de etiqueta de base de datos a un objeto de dominio.
    /// </summary>
    /// <param name="input">Entidad de base de datos.</param>
    /// <returns>Objeto de dominio de etiqueta.</returns>
    public TagDomain Map(Tag input) =>
        new()
        {
            Id = input.TagId,
            Text = input.Text,
            CreatedAt = input.CreatedAt,
        };
}
