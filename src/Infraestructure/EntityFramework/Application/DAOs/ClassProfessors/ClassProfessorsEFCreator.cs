using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public class ClassProfessorEFCreator(
    EduZasDotnetContext ctx,
    IMapper<ClassProfessor, ProfessorClassRelationDTO> domainMapper,
    IMapper<ProfessorClassRelationDTO, ClassProfessor> newEntityMapper
)
    : EFCreator<ProfessorClassRelationDTO, ProfessorClassRelationDTO, ClassProfessor>(
        ctx,
        domainMapper,
        newEntityMapper
    );
