using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

/// <summary>
/// Mapeador de creación para profesores de clase.
/// </summary>
public class NewClassProfessorEFMapper : IMapper<NewClassProfessorDTO, ClassProfessor>
{
    /// <inheritdoc/>
    public ClassProfessor Map(NewClassProfessorDTO r) =>
        new()
        {
            ClassId = r.ClassId,
            ProfessorId = r.UserId,
            IsOwner = r.IsOwner,
        };
}
