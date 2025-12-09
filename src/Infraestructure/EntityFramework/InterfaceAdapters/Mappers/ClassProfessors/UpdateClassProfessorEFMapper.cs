using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

/// <summary>
/// Mapeador de actualización para profesores de clase.
/// </summary>
public class UpdateClassProfessorEFMapper : IUpdateMapper<ClassProfessorUpdateDTO, ClassProfessor>
{
    /// <inheritdoc/>
    public void Map(ClassProfessorUpdateDTO uProps, ClassProfessor entity)
    {
        entity.ClassId = uProps.ClassId;
        entity.ProfessorId = uProps.UserId;
        entity.IsOwner = uProps.IsOwner;
    }
}
