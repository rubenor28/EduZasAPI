using System.Linq.Expressions;
using Application.DTOs.Tags;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tags;

/// <summary>
/// Proyector de consultas para etiquetas.
/// </summary>
public class TagProjector : IEFProjector<Tag, TagDomain, TagCriteriaDTO>
{
    /// <summary>
    /// Obtiene la expresión de proyección para convertir una entidad de etiqueta a un objeto de dominio.
    /// </summary>
    /// <param name="criteria">Criterios de consulta.</param>
    /// <returns>Expresión de proyección.</returns>
    public Expression<Func<Tag, TagDomain>> GetProjection(TagCriteriaDTO criteria) =>
        input =>
            new()
            {
                Id = input.TagId,
                Text = input.Text,
                CreatedAt = input.CreatedAt,
            };
}
