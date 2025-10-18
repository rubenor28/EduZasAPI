using Application.DTOs.ClassStudents;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassStudents;

public class ClassStudentEFCreator(
    EduZasDotnetContext ctx,
    IMapper<ClassStudent, StudentClassRelationDTO> domainMapper,
    IMapper<StudentClassRelationDTO, ClassStudent> newEntityMapper
)
    : EFCreator<StudentClassRelationDTO, StudentClassRelationDTO, ClassStudent>(
        ctx,
        domainMapper,
        newEntityMapper
    );
