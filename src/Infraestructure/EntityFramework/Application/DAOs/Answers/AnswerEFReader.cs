using System.Linq.Expressions;
using Application.DTOs.Answers;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Answers;

public sealed class AnswerEFReader(EduZasDotnetContext ctx, IMapper<Answer, AnswerDomain> mapper)
    : EFReader<AnswerIdDTO, AnswerDomain, Answer>(ctx, mapper)
{
    protected override Expression<Func<Answer, bool>> GetIdPredicate(AnswerIdDTO id) =>
        answer =>
            answer.UserId == id.UserId
            && answer.TestId == id.TestId
            && answer.ClassId == id.ClassId;
}
