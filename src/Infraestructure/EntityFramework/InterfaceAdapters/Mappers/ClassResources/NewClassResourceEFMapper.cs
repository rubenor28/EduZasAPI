using Application.DTOs.ClassResources;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassResources;

/// <summary>
/// Mapeador de creación para recursos de clase.
/// </summary>
public sealed class NewClassResourceEFMapper : IMapper<ClassResourceDTO, ClassResource>
{
    /// <summary>
    /// Mapea un DTO de nueva asociación recurso-clase a una entidad de base de datos.
    /// </summary>
    /// <param name="input">DTO de creación.</param>
    /// <returns>Entidad de base de datos.</returns>
    public ClassResource Map(ClassResourceDTO input) =>
        new()
        {
            ResourceId = input.ResourceId,
            ClassId = input.ClassId,
            Hidden = input.Hidden,
        };
}
