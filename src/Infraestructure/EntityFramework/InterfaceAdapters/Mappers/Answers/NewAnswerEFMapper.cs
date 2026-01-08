using Application.DTOs.Answers;
using Domain.Entities.QuestionAnswers;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Answers;

public sealed class NewAnswerEFMapper : IMapper<AnswerIdDTO, Answer>
{
    public Answer Map(AnswerIdDTO input) =>
        new()
        {
            ClassId = input.ClassId,
            TestId = input.TestId,
            UserId = input.UserId,
            Content = new Dictionary<Guid, IQuestionAnswer>(),
            Metadata = new()
        };
}
