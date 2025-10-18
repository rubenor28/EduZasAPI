using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public class ClassProfessorsEFQuerier(
    EduZasDotnetContext ctx,
    IMapper<ClassProfessor, ProfessorClassRelationDTO> domainMapper,
    int pageSize
)
    : EFQuerier<ProfessorClassRelationDTO, ProfessorClassRelationCriteriaDTO, ClassProfessor>(
        ctx,
        domainMapper,
        pageSize
    )
{
    public override IQueryable<ClassProfessor> BuildQuery(
        ProfessorClassRelationCriteriaDTO criteria
    ) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereOptional(criteria.UserId, userId => cr => cr.ProfessorId == userId)
            .WhereOptional(criteria.ClassId, classId => cr => cr.ClassId == classId)
            .WhereOptional(criteria.IsOwner, isOwner => cr => cr.IsOwner == isOwner);
}
