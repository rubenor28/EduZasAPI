using Application.DTOs.ClassStudents;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassStudents;

public class ClassStudentEFCreator(
    EduZasDotnetContext ctx,
    IMapper<ClassStudent, ClassStudentDomain> domainMapper,
    IMapper<NewClassStudentDTO, ClassStudent> newEntityMapper
)
    : EFCreator<ClassStudentDomain, NewClassStudentDTO, ClassStudent>(
        ctx,
        domainMapper,
        newEntityMapper
    );
