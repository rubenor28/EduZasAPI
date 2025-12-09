using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassStudents;

/// <summary>
/// Implementación de creación de relaciones Clase-Estudiante usando EF.
/// </summary>
public class ClassStudentEFCreator(
    EduZasDotnetContext ctx,
    IMapper<ClassStudent, ClassStudentDomain> domainMapper,
    IMapper<UserClassRelationId, ClassStudent> newEntityMapper
)
    : EFCreator<ClassStudentDomain, UserClassRelationId, ClassStudent>(
        ctx,
        domainMapper,
        newEntityMapper
    );
