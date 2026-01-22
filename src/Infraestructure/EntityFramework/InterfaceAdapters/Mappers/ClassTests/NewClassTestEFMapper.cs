using Application.DTOs.ClassTests;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

/// <summary>
/// Mapeador de creación para exámenes de clase.
/// </summary>
public class NewClassTestEFMapper : IMapper<ClassTestIdDTO, TestPerClass>
{
    /// <inheritdoc/>
    public TestPerClass Map(ClassTestIdDTO source) =>
        new()
        {
            TestId = source.TestId,
            ClassId = source.ClassId,
            AllowModifyAnswers = true,
        };
}
