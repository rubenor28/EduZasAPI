using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

public class TestMapper : IMapper<Test, TestDomain>
{
    public TestDomain Map(Test t) =>
        new()
        {
            Id = t.TestId,
            Active = t.Active,
            Title = t.Title,
            Content = t.Content,
            TimeLimitMinutes = t.TimeLimitMinutes,
            ProfessorId = t.ProfessorId,
            CreatedAt = t.CreatedAt,
            ModifiedAt = t.ModifiedAt,
        };
}
