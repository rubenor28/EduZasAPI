using System.Linq.Expressions;
using Application.DTOs.ClassTests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

/// <summary>
/// Proyector de consultas para asociaciones de exámenes de clase.
/// </summary>
public class ClassTestAssociationProjector
    : IEFProjector<Class, ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO>
{
    /// <summary>
    /// Obtiene la expresión de proyección para convertir una clase en un DTO de asociación de examen.
    /// </summary>
    /// <param name="criteria">Criterios de consulta que incluyen el ID del examen.</param>
    /// <returns>Expresión de proyección.</returns>
    public Expression<Func<Class, ClassTestAssociationDTO>> GetProjection(
        ClassTestAssociationCriteriaDTO criteria
    ) =>
        c =>
            new()
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                IsAssociated = c.TestsPerClasses.Any(tpc => tpc.TestId == criteria.TestId),
            };
}
