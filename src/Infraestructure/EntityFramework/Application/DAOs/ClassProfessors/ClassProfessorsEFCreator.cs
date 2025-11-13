using Application.DTOs.ClassProfessors;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public class ClassProfessorsEFCreator(
    EduZasDotnetContext ctx,
    IMapper<ClassProfessor, ClassProfessorDomain> domainMapper,
    IMapper<NewClassProfessorDTO, ClassProfessor> newEntityMapper
)
    : EFCreator<ClassProfessorDomain, NewClassProfessorDTO, ClassProfessor>(
        ctx,
        domainMapper,
        newEntityMapper
    );
