using Application.DTOs.Classes;
using Application.DTOs.ClassStudents;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassStudents;

public class ClassStudentsEFDeleter
    : CompositeKeyEFDeleter<ClassUserRelationIdDTO, StudentClassRelationDTO, ClassStudent>
{
    public ClassStudentsEFDeleter(
        EduZasDotnetContext ctx,
        IMapper<ClassStudent, StudentClassRelationDTO> domainMapper
    )
        : base(ctx, domainMapper) { }

    public override async Task<ClassStudent?> GetTrackedById(ClassUserRelationIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.StudentId == id.UserId)
            .Where(cs => cs.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
