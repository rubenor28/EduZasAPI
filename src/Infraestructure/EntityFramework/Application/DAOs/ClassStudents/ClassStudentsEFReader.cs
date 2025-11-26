using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassStudents;

public class ClassStudentsEFReader(
    EduZasDotnetContext ctx,
    IEFProjector<ClassStudent, ClassStudentDomain> projector
) : EFReader<UserClassRelationId, ClassStudentDomain, ClassStudent>(ctx, projector)
{
    protected override Expression<Func<ClassStudent, bool>> GetIdPredicate(
        UserClassRelationId id
    ) => cs => cs.ClassId == id.ClassId && cs.StudentId == id.UserId;
}
