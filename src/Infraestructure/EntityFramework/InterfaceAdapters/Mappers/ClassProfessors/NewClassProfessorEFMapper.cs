using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

/// <summary>
/// Mapeador de creación para profesores de clase.
/// </summary>
public class NewClassProfessorEFMapper : IMapper<NewClassProfessorDTO, ClassProfessor>
{
    /// <summary>
    /// Mapea un DTO de nueva relación profesor-clase a una entidad de base de datos.
    /// </summary>
    /// <param name="source">DTO de creación.</param>
    /// <returns>Entidad de base de datos.</returns>
    public ClassProfessor Map(NewClassProfessorDTO source) =>
        new()
        {
            ClassId = source.ClassId,
            ProfessorId = source.UserId,
            IsOwner = source.IsOwner,
        };
}
