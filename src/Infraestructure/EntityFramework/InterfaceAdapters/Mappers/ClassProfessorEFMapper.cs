using Application.DTOs.ClassProfessors;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class ClassProfessorEFMapper
    : IMapper<ClassProfessor, ClassProfessorDomain>,
        IMapper<NewClassProfessorDTO, ClassProfessor>,
        IUpdateMapper<ClassProfessorUpdateDTO, ClassProfessor>
{
    public ClassProfessor Map(NewClassProfessorDTO r) =>
        new()
        {
            ClassId = r.ClassId,
            ProfessorId = r.UserId,
            IsOwner = r.IsOwner,
        };

    public ClassProfessorDomain Map(ClassProfessor efEntity) =>
        new()
        {
            Id = new() { ClassId = efEntity.ClassId, UserId = efEntity.ProfessorId },
            IsOwner = efEntity.IsOwner ?? false,
            CreatedAt = efEntity.CreatedAt,
        };

    public void Map(ClassProfessorUpdateDTO uProps, ClassProfessor entity)
    {
        entity.ClassId = uProps.Id.ClassId;
        entity.ProfessorId = uProps.Id.UserId;
        entity.IsOwner = uProps.IsOwner;
    }
}
