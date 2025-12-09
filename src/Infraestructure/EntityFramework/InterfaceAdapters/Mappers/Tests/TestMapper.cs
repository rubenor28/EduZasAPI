using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

/// <summary>
/// Mapeador de entidad EF a dominio para exámenes.
/// </summary>
public class TestMapper : IMapper<Test, TestDomain>
{
    /// <inheritdoc/>
    public TestDomain Map(Test t) =>
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
