using Application.DTOs.Tests;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

/// <summary>
/// Mapeador de creación para exámenes.
/// </summary>
public class NewTestEFMapper : IMapper<NewTestDTO, Test>
{
    /// <inheritdoc/>
    public Test Map(NewTestDTO nt) =>
        new()
        {
            Title = nt.Title,
            Content = nt.Content,
            TimeLimitMinutes = nt.TimeLimitMinutes,
            ProfessorId = nt.ProfessorId,
        };
}
