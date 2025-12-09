using Application.DTOs.Resources;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Resources;

/// <summary>
/// Mapeador de creación para recursos.
/// </summary>
public class NewResourceEFMapper : IMapper<NewResourceDTO, Resource>
{
    /// <inheritdoc/>
    public Resource Map(NewResourceDTO input) =>
        new()
        {
            ResourceId = Guid.NewGuid(),
            Title = input.Title,
            Content = input.Content,
            ProfessorId = input.ProfessorId,
        };
}
