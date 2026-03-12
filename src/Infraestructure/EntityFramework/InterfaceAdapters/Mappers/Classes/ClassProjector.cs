using System.Linq.Expressions;
using Application.DTOs.Classes;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

/// <summary>
/// Proyector de consultas para clases.
/// </summary>
public class ClassProjector : IEFProjector<Class, ClassDomain, ClassCriteriaDTO>
{
    /// <summary>
    /// Obtiene la expresión de proyección para convertir una entidad de clase de base de datos a un objeto de dominio.
    /// </summary>
    /// <param name="criteria">Criterios de consulta.</param>
    /// <returns>Expresión de proyección.</returns>
    public Expression<Func<Class, ClassDomain>> GetProjection(ClassCriteriaDTO criteria) =>
        ef =>
            new()
            {
                Id = ef.ClassId,
                Active = ef.Active ?? false,
                ClassName = ef.ClassName,
                Color = ef.Color ?? "#007bff",
                Subject = ef.Subject,
                Section = ef.Section,
                CreatedAt = ef.CreatedAt,
                ModifiedAt = ef.ModifiedAt,
            };
}
