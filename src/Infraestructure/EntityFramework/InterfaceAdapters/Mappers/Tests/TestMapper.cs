using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

/// <summary>
/// Mapeador de entidad EF a dominio para exámenes.
/// </summary>
public class TestMapper : IMapper<Test, TestDomain>
{
    /// <summary>
    /// Mapea una entidad de examen de base de datos a un objeto de dominio.
    /// </summary>
    /// <param name="input">Entidad de base de datos.</param>
    /// <returns>Objeto de dominio de examen.</returns>
    public TestDomain Map(Test input) =>
        new()
        {
            Id = t.TestId,
            Active = t.Active,
            Title = t.Title,
            Color = t.Color,
            Content = t.Content,
            TimeLimitMinutes = t.TimeLimitMinutes,
            ProfessorId = t.ProfessorId,
            CreatedAt = t.CreatedAt,
            ModifiedAt = t.ModifiedAt,
        };
}
