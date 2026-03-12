using Application.DTOs.Resources;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Resources;

/// <summary>
/// Mapeador de creación para recursos.
/// </summary>
public class NewResourceEFMapper : IMapper<NewResourceDTO, Resource>
{
    /// <summary>
    /// Mapea un DTO de nuevo recurso a una entidad de base de datos.
    /// </summary>
    /// <param name="input">DTO con los datos del nuevo recurso.</param>
    /// <returns>Entidad de recurso para persistencia.</returns>
    public Resource Map(NewResourceDTO input) =>
        new()
        {
            ResourceId = Guid.NewGuid(),
            Title = input.Title,
            Content = input.Content,
            ProfessorId = input.ProfessorId,
        };
}
