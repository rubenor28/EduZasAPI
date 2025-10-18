using Application.DTOs.ClassStudents;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassStudents;

public class ClassStudentEFQuerier(
    EduZasDotnetContext ctx,
    IMapper<ClassStudent, StudentClassRelationDTO> domainMapper,
    int pageSize
)
    : EFQuerier<StudentClassRelationDTO, StudentClassRelationCriteriaDTO, ClassStudent>(
        ctx,
        domainMapper,
        pageSize
    )
{
    public override IQueryable<ClassStudent> BuildQuery(StudentClassRelationCriteriaDTO criteria) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereOptional(criteria.UserId, userId => cr => cr.StudentId == userId)
            .WhereOptional(criteria.ClassId, classId => cr => cr.ClassId == classId)
            .WhereOptional(criteria.Hidden, hidden => cr => cr.Hidden == hidden);
}
