using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public class ClassProfessorsEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<ClassProfessor, ProfessorClassRelationDTO> domainMapper,
    IUpdateMapper<ProfessorClassRelationDTO, ClassProfessor> updateMapper
)
    : RelationEFUpdater<ClassUserRelationIdDTO, ProfessorClassRelationDTO, ClassProfessor>(
        ctx,
        domainMapper,
        updateMapper
    )
{
    protected override async Task<ClassProfessor?> GetTrackedById(ClassUserRelationIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.ProfessorId == id.UserId)
            .Where(cs => cs.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
