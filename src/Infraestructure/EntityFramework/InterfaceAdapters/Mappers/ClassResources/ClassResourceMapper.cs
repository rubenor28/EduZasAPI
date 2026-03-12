using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassResources;

/// <summary>
/// Mapeador de entidad EF a dominio para recursos de clase.
/// </summary>
public class ClassResourceMapper : IMapper<ClassResource, ClassResourceDomain>
{
    /// <summary>
    /// Mapea una entidad de recurso de clase a un objeto de dominio.
    /// </summary>
    /// <param name="input">Entidad de base de datos.</param>
    /// <returns>Objeto de dominio.</returns>
    public ClassResourceDomain Map(ClassResource input) =>
        new()
        {
            ClassId = input.ClassId,
            ResourceId = input.ResourceId,
            Hidden = input.Hidden,
            CreatedAt = input.CreatedAt,
        };
}
