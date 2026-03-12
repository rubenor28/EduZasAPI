using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Resources;

/// <summary>
/// Mapeador de entidad EF a dominio para recursos.
/// </summary>
public sealed class ResourceMapper : IMapper<Resource, ResourceDomain>
{
    /// <summary>
    /// Mapea una entidad de recurso de base de datos a un objeto de dominio.
    /// </summary>
    /// <param name="source">Entidad de base de datos.</param>
    /// <returns>Objeto de dominio de recurso.</returns>
    public ResourceDomain Map(Resource source) =>
        new()
        {
            Id = input.ResourceId,
            Color = input.Color,
            Active = input.Active ?? false,
            Content = input.Content,
            Title = input.Title,
            CreatedAt = input.CreatedAt,
            ModifiedAt = input.ModifiedAt,
            ProfessorId = input.ProfessorId,
        };
}
