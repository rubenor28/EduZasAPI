using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class ProfessorClassEFMapper
    : IMapper<ClassProfessor, ProfessorClassRelationDTO>,
        IMapper<ProfessorClassRelationDTO, ClassProfessor>,
        IUpdateMapper<ProfessorClassRelationDTO, ClassProfessor>
{
    public ClassProfessor Map(ProfessorClassRelationDTO r) =>
        new()
        {
            ClassId = r.Id.ClassId,
            ProfessorId = r.Id.UserId,
            IsOwner = r.IsOwner,
        };

    public ProfessorClassRelationDTO Map(ClassProfessor efEntity) =>
        new()
        {
            Id = new() { ClassId = efEntity.ClassId, UserId = efEntity.ProfessorId },
            IsOwner = efEntity.IsOwner ?? false,
        };

    public void Map(ProfessorClassRelationDTO uProps, ClassProfessor entity)
    {
        entity.ClassId = uProps.Id.ClassId;
        entity.ProfessorId = uProps.Id.UserId;
        entity.IsOwner = uProps.IsOwner;
    }
}
