using Application.DTOs.Tests;
using Domain.Entities.PublicQuestions;
using Domain.Entities.Questions;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

public class PublicTestMapper(IMapper<IQuestion, IPublicQuestion> questionMapper)
    : IMapper<Test, PublicTestDTO>
{
    private readonly IMapper<IQuestion, IPublicQuestion> _questionMapper = questionMapper;

    public PublicTestDTO Map(Test input) =>
        new()
        {
            Id = input.TestId,
            Active = input.Active,
            ProfessorId = input.ProfessorId,
            Title = input.Title,
            Color = input.Color,
            TimeLimitMinutes = input.TimeLimitMinutes,
            Content = input.Content.Select(q => new PublicQuestionDTO
            {
                Id = q.Key,
                Data = _questionMapper.Map(q.Value),
            }),
        };
}
