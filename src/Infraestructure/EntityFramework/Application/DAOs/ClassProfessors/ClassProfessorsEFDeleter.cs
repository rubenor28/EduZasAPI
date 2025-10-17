using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public class ClassProfessorsEFDeleter
    : CompositeKeyEFDeleter<ClassUserRelationIdDTO, ProfessorClassRelationDTO, ClassProfessor>
{
    public ClassProfessorsEFDeleter(
        EduZasDotnetContext ctx,
        IMapper<ClassProfessor, ProfessorClassRelationDTO> domainMapper
    )
        : base(ctx, domainMapper) { }

    public override async Task<ClassProfessor?> GetTrackedById(ClassUserRelationIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.ProfessorId == id.UserId)
            .Where(cs => cs.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
