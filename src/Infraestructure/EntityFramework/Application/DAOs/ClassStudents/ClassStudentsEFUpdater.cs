using Application.DTOs.ClassStudents;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassStudents;

public class ClassStudentsEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<ClassStudent, ClassStudentDomain> domainMapper,
    IUpdateMapper<ClassStudentUpdateDTO, ClassStudent> updateMapper
)
    : EFUpdater<ClassStudentDomain, ClassStudentUpdateDTO, ClassStudent>(
        ctx,
        domainMapper,
        updateMapper
    )
{
    protected override async Task<ClassStudent?> GetTrackedByDTO(ClassStudentUpdateDTO value) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.StudentId == value.UserId)
            .Where(cs => cs.ClassId == value.ClassId)
            .FirstOrDefaultAsync();
}
