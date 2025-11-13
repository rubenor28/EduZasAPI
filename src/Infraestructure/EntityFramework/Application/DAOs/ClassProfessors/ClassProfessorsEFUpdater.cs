using Application.DTOs.ClassProfessors;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public class ClassProfessorsEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<ClassProfessor, ClassProfessorDomain> domainMapper,
    IUpdateMapper<ClassProfessorUpdateDTO, ClassProfessor> updateMapper
)
    : CompositeKeyEFUpdater<
        UserClassRelationId,
        ClassProfessorDomain,
        ClassProfessorUpdateDTO,
        ClassProfessor
    >(ctx, domainMapper, updateMapper)
{
    protected override async Task<ClassProfessor?> GetTrackedById(UserClassRelationId id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.ProfessorId == id.UserId)
            .Where(cs => cs.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
