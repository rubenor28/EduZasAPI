using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public class ClassProfessorEFCreator
    : EFCreator<ProfessorClassRelationDTO, ProfessorClassRelationDTO, ClassProfessor>
{
    public ClassProfessorEFCreator(
        EduZasDotnetContext ctx,
        IMapper<ClassProfessor, ProfessorClassRelationDTO> domainMapper,
        IMapper<ProfessorClassRelationDTO, ClassProfessor> newEntityMapper
    )
        : base(ctx, domainMapper, newEntityMapper) { }
}
