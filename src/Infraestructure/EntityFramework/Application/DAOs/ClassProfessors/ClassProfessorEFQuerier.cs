using Application.DTOs.ClassProfessors;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public sealed class ClassProfessorEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<ClassProfessor, ClassProfessorDomain> projector,
    int pageSize
)
    : EFQuerier<ClassProfessorDomain, ClassProfessorCriteriaDTO, ClassProfessor>(
        ctx,
        projector,
        pageSize
    )
{
    public override IQueryable<ClassProfessor> BuildQuery(ClassProfessorCriteriaDTO c) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereOptional(c.UserId, id => cp => cp.ProfessorId == id)
            .WhereOptional(c.ClassId, id => cp => cp.ClassId == id);
}
