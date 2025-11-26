using Application.DTOs.Tests;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

public class NewTestEFMapper : IMapper<NewTestDTO, Test>
{
    public Test Map(NewTestDTO nt) =>
        new()
        {
            Title = nt.Title,
            Content = nt.Content,
            TimeLimitMinutes = nt.TimeLimitMinutes.ToNullable(),
            ProfessorId = nt.ProfessorId,
        };
}
