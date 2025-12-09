using Application.DTOs.ClassResources;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassResources;

public sealed class ClassResourceAssociationEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Class, ClassResourceAssociationDTO, ClassResourceAssociationCriteriaDTO> projector,
    int maxPageSize
)
    : EFQuerier<ClassResourceAssociationDTO, ClassResourceAssociationCriteriaDTO, Class>(
        ctx,
        projector,
        maxPageSize
    )
{
    public override IQueryable<Class> BuildQuery(ClassResourceAssociationCriteriaDTO criteria) =>
        _dbSet
            .AsNoTracking()
            .Where(c => c.ClassProfessors.Any(p => p.ProfessorId == criteria.ProfessorId))
            .OrderByDescending(c =>
                c.ClassProfessors.Where(cp => cp.ProfessorId == criteria.ProfessorId)
                    .Max(c => c.CreatedAt)
            );
}