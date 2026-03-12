using Application.DTOs.Tests;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

/// <summary>
/// Mapeador de creación para exámenes.
/// </summary>
public class NewTestEFMapper : IMapper<NewTestDTO, Test>
{
    /// <summary>
    /// Mapea un DTO de nuevo examen a una entidad de base de datos.
    /// </summary>
    /// <param name="nt">DTO de creación.</param>
    /// <returns>Entidad de base de datos.</returns>
    public Test Map(NewTestDTO nt) =>
        new()
        {
            Title = nt.Title,
            Content = nt.Content,
            TimeLimitMinutes = nt.TimeLimitMinutes,
            ProfessorId = nt.ProfessorId,
        };
}
