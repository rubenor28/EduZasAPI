using Application.DTOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

public class UpdateTestEFMapper : IUpdateMapper<TestUpdateDTO, Test>
{
    public void Map(TestUpdateDTO tu, Test t)
    {
        t.Title = tu.Title;
        t.Content = tu.Content;
        t.TimeLimitMinutes = tu.TimeLimitMinutes;
        t.ProfessorId = tu.ProfessorId;
        t.Active = tu.Active;
    }
}
