using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassResources;

/// <summary>
/// Mapeador de entidad EF a dominio para recursos de clase.
/// </summary>
public class ClassResourceMapper : IMapper<ClassResource, ClassResourceDomain>
{
    /// <inheritdoc/>
    public ClassResourceDomain Map(ClassResource input) =>
        new()
        {
            ClassId = input.ClassId,
            ResourceId = input.ResourceId,
            Hidden = input.Hidden,
            CreatedAt = input.CreatedAt,
        };
}
