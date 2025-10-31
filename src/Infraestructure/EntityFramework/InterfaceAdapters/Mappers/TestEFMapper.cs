using Application.DTOs.Tests;
using Domain.Entities;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public sealed class TestEFMapper
    : IMapper<Test, TestDomain>,
        IMapper<NewTestDTO, Test>,
        IUpdateMapper<TestUpdateDTO, Test>
{
    public TestDomain Map(Test input) =>
        new()
        {
            Id = input.TestId,
            Title = input.Title,
            Content = input.Content,
            TimeLimitMinutes = input.TimeLimitMinutes.ToOptional(),
            ProfesorId = input.ProfessorId,
            CreatedAt = input.CreatedAt,
            ModifiedAt = input.ModifiedAt,
        };

    public Test Map(NewTestDTO input) =>
        new()
        {
            Title = input.Title,
            Content = input.Content,
            TimeLimitMinutes = input.TimeLimitMinutes.ToNullable(),
            ProfessorId = input.ProfesorId,
        };

    public void Map(TestUpdateDTO source, Test destination)
    {
        destination.Title = source.Title;
        destination.Content = source.Content;
        destination.TimeLimitMinutes = source.TimeLimitMinutes.ToNullable();
        destination.ProfessorId = source.ProfesorId;
    }
}
