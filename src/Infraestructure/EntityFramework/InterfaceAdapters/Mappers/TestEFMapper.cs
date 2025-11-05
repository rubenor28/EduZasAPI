using Application.DTOs.Tests;
using Domain.Entities;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class TestEFMapper
    : IMapper<NewTestDTO, Test>,
        IMapper<Test, TestDomain>,
        IUpdateMapper<TestUpdateDTO, Test>
{
    public Test Map(NewTestDTO nt) =>
        new()
        {
            Title = nt.Title,
            Content = nt.Content,
            TimeLimitMinutes = nt.TimeLimitMinutes.ToNullable(),
            ProfessorId = nt.ProfesorId
        };

    public TestDomain Map(Test t) =>
        new()
        {
            Id = t.TestId,
            Title = t.Title,
            Content = t.Content,
            TimeLimitMinutes = t.TimeLimitMinutes.ToOptional(),
            ProfessorId = t.ProfessorId,
            CreatedAt = t.CreatedAt,
            ModifiedAt = t.ModifiedAt
        };

    public void Map(TestUpdateDTO tu, Test t)
    {
        t.Title = tu.Title;
        t.Content = tu.Content;
        t.TimeLimitMinutes = tu.TimeLimitMinutes.ToNullable();
        t.ProfessorId = tu.ProfesorId;
    }
}
