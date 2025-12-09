using System.Linq.Expressions;
using Application.DTOs.ClassTests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

/// <summary>
/// Proyector de consultas para asociaciones de exámenes de clase.
/// </summary>
public class ClassTestAssociationProjector : IEFProjector<Class, ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO>
{
    /// <inheritdoc/>
    public Expression<Func<Class, ClassTestAssociationDTO>> GetProjection(ClassTestAssociationCriteriaDTO criteria) =>
        c =>
            new()
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                IsAssociated = c.TestsPerClasses.Any(tpc => tpc.TestId == criteria.TestId),
                IsVisible = c.TestsPerClasses
                    .Where(tpc => tpc.TestId == criteria.TestId)
                    .Select(tpc => tpc.Visible)
                    .FirstOrDefault()
            };
}
