using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Resources;

/// <summary>
/// Mapeador de entidad EF a dominio para recursos.
/// </summary>
public sealed class ResourceMapper : IMapper<Resource, ResourceDomain>
{
    /// <inheritdoc/>
    public ResourceDomain Map(Resource input) =>
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
