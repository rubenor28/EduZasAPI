using Application.DTOs.ClassResources;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassResources;

/// <summary>
/// Mapeador de actualización para recursos de clase.
/// </summary>
public sealed class ClassResourceUpdateMapper : IUpdateMapper<ClassResourceDTO, ClassResource>
{
    /// <summary>
    /// Actualiza una entidad de asociación recurso-clase con los datos del DTO.
    /// </summary>
    /// <param name="s">DTO de actualización.</param>
    /// <param name="d">Entidad de base de datos.</param>
    public void Map(ClassResourceDTO s, ClassResource d)
    {
        d.Hidden = s.Hidden;
    }
}
