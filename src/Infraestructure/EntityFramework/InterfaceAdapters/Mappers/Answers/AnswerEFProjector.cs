using System.Linq.Expressions;
using Application.DTOs.Answers;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Answers;

public class AnswerEFProjector : IEFProjector<Answer, AnswerDomain, AnswerCriteriaDTO>
{
    public Expression<Func<Answer, AnswerDomain>> GetProjection(AnswerCriteriaDTO criteria) =>
        answer =>
            new()
            {
                UserId = answer.UserId,
                ClassId = answer.ClassId,
                TestId = answer.TestId,
                TryFinished = answer.TryFinished,
                Metadata = answer.Metadata,
                Content = answer.Content,
                ModifiedAt = answer.ModifiedAt,
                CreatedAt = answer.CreatedAt,
            };
}
