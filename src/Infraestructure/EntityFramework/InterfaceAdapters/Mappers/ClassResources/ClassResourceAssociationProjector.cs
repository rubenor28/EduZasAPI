using System.Linq.Expressions;
using Application.DTOs.ClassResources;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassResources;

/// <summary>
/// Proyector de consultas para asociaciones de recursos de clase.
/// </summary>
public class ClassResourceAssociationProjector : IEFProjector<Class, ClassResourceAssociationDTO, ClassResourceAssociationCriteriaDTO>
{
    /// <inheritdoc/>
    public Expression<Func<Class, ClassResourceAssociationDTO>> GetProjection(ClassResourceAssociationCriteriaDTO criteria) =>
        c =>
            new()
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                IsAssociated = c.ClassResources.Any(cr => cr.ResourceId == criteria.ResourceId),
                IsHidden = c.ClassResources
                    .Where(cr => cr.ResourceId == criteria.ResourceId)
                    .Select(cr => cr.Hidden)
                    .FirstOrDefault()
            };
}
