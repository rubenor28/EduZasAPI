using Application.DTOs.ClassStudents;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassStudents;

public class ClassStudentEFCreator
    : EFCreator<StudentClassRelationDTO, StudentClassRelationDTO, ClassStudent>
{
    public ClassStudentEFCreator(
        EduZasDotnetContext ctx,
        IMapper<ClassStudent, StudentClassRelationDTO> domainMapper,
        IMapper<StudentClassRelationDTO, ClassStudent> newEntityMapper
    )
        : base(ctx, domainMapper, newEntityMapper) { }
}
