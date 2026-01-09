using Application.DTOs.Tests;
using Domain.Entities.PublicQuestions;
using Domain.Entities.Questions;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

public class PublicTestMapper(IMapper<Guid, IQuestion, IPublicQuestion> questionMapper)
    : IMapper<Test, PublicTestDTO>
{
    private readonly IMapper<Guid, IQuestion, IPublicQuestion> _questionMapper = questionMapper;

    public PublicTestDTO Map(Test input) =>
        new()
        {
            Id = input.TestId,
            Active = input.Active,
            ProfessorId = input.ProfessorId,
            Title = input.Title,
            Color = input.Color,
            TimeLimitMinutes = input.TimeLimitMinutes,
            Content = input.Content.Select(q => _questionMapper.Map(q.Key, q.Value)),
        };
}
