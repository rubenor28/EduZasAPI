using Application.DTOs.ClassProfessors;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassProfessors;

/// <summary>
/// Implementación de consulta de relaciones Clase-Profesor usando EF.
/// </summary>
public sealed class ClassProfessorEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<ClassProfessor, ClassProfessorDomain, ClassProfessorCriteriaDTO> projector,
    int maxPageSize
)
    : EFQuerier<ClassProfessorDomain, ClassProfessorCriteriaDTO, ClassProfessor>(
        ctx,
        projector,
        maxPageSize
    )
{
    /// <inheritdoc/>
    public override IQueryable<ClassProfessor> BuildQuery(ClassProfessorCriteriaDTO c) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereOptional(c.UserId, id => cp => cp.ProfessorId == id)
            .WhereOptional(c.ClassId, id => cp => cp.ClassId == id);
}
