using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;

public class AnswerEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Answer, AnswerDomain, AnswerCriteriaDTO> projector,
    int maxPageSize
) : EFQuerier<AnswerDomain, AnswerCriteriaDTO, Answer>(ctx, projector, maxPageSize)
{
    public override IQueryable<Answer> BuildQuery(AnswerCriteriaDTO criteria) =>
        _dbSet
            .WhereOptional(criteria.UserId, id => ef => ef.UserId == id)
            .WhereOptional(criteria.TestId, id => ef => ef.TestId == id)
            .WhereOptional(criteria.ClassId, id => ef => ef.ClassId == id)
            .WhereOptional(
                criteria.TestOwnerId,
                id => ef => ef.TestPerClass.Test.ProfessorId == id
            );
}
