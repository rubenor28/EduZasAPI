using Application.DTOs.Classes;
using Application.DTOs.ClassStudents;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassStudents;

public class ClassStudentEFCreator(
    EduZasDotnetContext ctx,
    IMapper<ClassStudent, StudentClassRelationDTO> domainMapper,
    IMapper<EnrollClassDTO, ClassStudent> newEntityMapper
)
    : EFCreator<StudentClassRelationDTO, EnrollClassDTO, ClassStudent>(
        ctx,
        domainMapper,
        newEntityMapper
    );
