using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

public class NewClassProfessorEFMapper : IMapper<NewClassProfessorDTO, ClassProfessor>
{
    public ClassProfessor Map(NewClassProfessorDTO r) =>
        new()
        {
            ClassId = r.ClassId,
            ProfessorId = r.UserId,
            IsOwner = r.IsOwner,
        };
}
