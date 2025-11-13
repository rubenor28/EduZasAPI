using Application.DTOs.ClassStudents;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassStudents;

public class ClassStudentsEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<ClassStudent, ClassStudentDomain> domainMapper,
    IUpdateMapper<ClassStudentUpdateDTO, ClassStudent> updateMapper
)
    : CompositeKeyEFUpdater<
        UserClassRelationId,
        ClassStudentDomain,
        ClassStudentUpdateDTO,
        ClassStudent
    >(ctx, domainMapper, updateMapper)
{
    protected override async Task<ClassStudent?> GetTrackedById(UserClassRelationId id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.StudentId == id.UserId)
            .Where(cs => cs.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
