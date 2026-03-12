using System.Linq.Expressions;
using Application.DTOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

/// <summary>
/// Proyector de consultas para resúmenes de exámenes.
/// </summary>
public class TestSummaryProjector : IEFProjector<Test, TestSummaryDTO, TestCriteriaDTO>
{
    /// <summary>
    /// Obtiene la expresión de proyección para convertir una entidad de examen en un resumen de examen.
    /// </summary>
    /// <param name="criteria">Criterios de consulta.</param>
    /// <returns>Expresión de proyección para el resumen.</returns>
    public Expression<Func<Test, TestSummaryDTO>> GetProjection(TestCriteriaDTO criteria) =>
        t =>
            new()
            {
                Id = t.TestId,
                Title = t.Title,
                Color = t.Color,
                Active = t.Active,
                ModifiedAt = t.ModifiedAt,
            };
}
