using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

public class UpdateClassProfessorEFMapper : IUpdateMapper<ClassProfessorUpdateDTO, ClassProfessor>
{
    public void Map(ClassProfessorUpdateDTO uProps, ClassProfessor entity)
    {
        entity.ClassId = uProps.ClassId;
        entity.ProfessorId = uProps.UserId;
        entity.IsOwner = uProps.IsOwner;
    }
}
