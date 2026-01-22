using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

/// <summary>
/// Mapeador de entidad EF a dominio para exámenes de clase.
/// </summary>
public class ClassTestMapper : IMapper<TestPerClass, ClassTestDomain>
{
    /// <inheritdoc/>
    public ClassTestDomain Map(TestPerClass source) =>
        new()
        {
            TestId = source.TestId,
            ClassId = source.ClassId,
            AllowModifyAnswers = source.AllowModifyAnswers,
            CreatedAt = source.CreatedAt,
        };
}
