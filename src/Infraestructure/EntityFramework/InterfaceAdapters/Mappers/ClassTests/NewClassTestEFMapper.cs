using Application.DTOs.ClassTests;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassTests;

/// <summary>
/// Mapeador de creación para exámenes de clase.
/// </summary>
public class NewClassTestEFMapper : IMapper<ClassTestIdDTO, TestPerClass>
{
    /// <summary>
    /// Mapea un DTO de nueva asociación examen-clase a una entidad de base de datos.
    /// </summary>
    /// <param name="source">DTO de creación.</param>
    /// <returns>Entidad de base de datos.</returns>
    public TestPerClass Map(ClassTestIdDTO source) =>
        new()
        {
            TestId = source.TestId,
            ClassId = source.ClassId,
            AllowModifyAnswers = true,
        };
}
