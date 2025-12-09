using Application.DTOs.Resources;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Resources;

/// <summary>
/// Mapeador de actualización para recursos.
/// </summary>
public class UpdateResourceEFMapper : IUpdateMapper<ResourceUpdateDTO, Resource>
{
    /// <inheritdoc/>
    public void Map(ResourceUpdateDTO s, Resource d)
    {
        d.Active = s.Active;
        d.Title = s.Title;
        d.Content = s.Content;
    }
}
