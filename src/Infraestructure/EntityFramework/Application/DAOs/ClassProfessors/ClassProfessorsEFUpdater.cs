using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public class ClassProfessorsEFUpdater
    : CompositeKeyEFUpdater<ClassUserRelationIdDTO, ProfessorClassRelationDTO, ClassProfessor>
{
    public ClassProfessorsEFUpdater(
        EduZasDotnetContext ctx,
        IMapper<ClassProfessor, ProfessorClassRelationDTO> domainMapper,
        IUpdateMapper<ProfessorClassRelationDTO, ClassProfessor> updateMapper
    )
        : base(ctx, domainMapper, updateMapper) { }

    public override async Task<ClassProfessor?> GetTrackedById(ClassUserRelationIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.ProfessorId == id.UserId)
            .Where(cs => cs.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
