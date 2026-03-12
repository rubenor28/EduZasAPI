using System.Linq.Expressions;
using Application.DTOs.Resources;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Resources;

/// <summary>
/// Proyector de consultas para recursos.
/// </summary>
public class ResourceProjector : IEFProjector<Resource, ResourceDomain, ResourceCriteriaDTO>
{
    /// <summary>
    /// Obtiene la expresión de proyección para convertir una entidad de recurso de base de datos a un objeto de dominio.
    /// </summary>
    /// <param name="criteria">Criterios de consulta.</param>
    /// <returns>Expresión de proyección.</returns>
    public Expression<Func<Resource, ResourceDomain>> GetProjection(ResourceCriteriaDTO criteria) =>
        input =>
            new()
            {
                Id = input.ResourceId,
                Active = input.Active ?? true,
                Color = input.Color,
                Content = input.Content,
                ProfessorId = input.ProfessorId,
                Title = input.Title,
                CreatedAt = input.CreatedAt,
                ModifiedAt = input.ModifiedAt,
            };
}

/// <summary>
/// Proyector de consultas para resúmenes de recursos.
/// </summary>
public class ResourceSummaryProjector : IEFProjector<Resource, ResourceSummary, ResourceCriteriaDTO>
{
    /// <summary>
    /// Obtiene la expresión de proyección para convertir una entidad de recurso a un resumen de recurso.
    /// </summary>
    /// <param name="criteria">Criterios de consulta.</param>
    /// <returns>Expresión de proyección para el resumen.</returns>
    public Expression<Func<Resource, ResourceSummary>> GetProjection(ResourceCriteriaDTO criteria) =>
        input =>
            new()
            {
                Id = input.ResourceId,
                Color = input.Color,
                Active = input.Active ?? false,
                ProfessorId = input.ProfessorId,
                Title = input.Title,
            };
}
