using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Answers;

public class AnswerMapper : IMapper<Answer, AnswerDomain>
{
    public AnswerDomain Map(Answer input) =>
        new()
        {
            ClassId = input.ClassId,
            UserId = input.UserId,
            TestId = input.TestId,
            TryFinished = input.TryFinished,
            Content = input.Content,
            Metadata = input.Metadata,
            CreatedAt = input.CreatedAt,
            ModifiedAt = input.ModifiedAt,
        };
}
