using Application.DTOs.ClassTests;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassTests;

/// <summary>
/// Implementación de consulta de asociaciones Clase-Examen usando EF.
/// </summary>
public sealed class ClassTestAssociationEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Class, ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO> projector,
    int maxPageSize
) : EFQuerier<ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO, Class>(ctx, projector, maxPageSize)
{
    /// <summary>
    /// Construye la consulta para obtener las asociaciones de exámenes de clase a partir de los criterios.
    /// </summary>
    /// <param name="c">Criterios de consulta.</param>
    /// <returns>IQueryable de clases.</returns>
    public override IQueryable<Class> BuildQuery(ClassTestAssociationCriteriaDTO criteria) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .Where(c => c.ClassProfessors.Any(cp => cp.ProfessorId == criteria.ProfessorId));
}
