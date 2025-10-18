using Application.DTOs.Classes;
using Application.DTOs.ClassStudents;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassStudents;

public class ClassStudentsEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<ClassStudent, StudentClassRelationDTO> domainMapper,
    IUpdateMapper<StudentClassRelationDTO, ClassStudent> updateMapper
)
    : CompositeKeyEFUpdater<ClassUserRelationIdDTO, StudentClassRelationDTO, ClassStudent>(
        ctx,
        domainMapper,
        updateMapper
    )
{
    public override async Task<ClassStudent?> GetTrackedById(ClassUserRelationIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.StudentId == id.UserId)
            .Where(cs => cs.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
