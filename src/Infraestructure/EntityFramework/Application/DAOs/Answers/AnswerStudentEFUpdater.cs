using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

public sealed class AnswerStudentEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<Answer, AnswerDomain> domainMapper,
    IUpdateMapper<AnswerUpdateStudentDTO, Answer> updateMapper
) : EFUpdater<AnswerDomain, AnswerUpdateStudentDTO, Answer>(ctx, domainMapper, updateMapper)
{
    protected override Task<Answer?> GetTrackedByDTO(AnswerUpdateStudentDTO value) =>
        _dbSet
            .Where(answer =>
                answer.ClassId == value.ClassId
                && answer.TestId == value.TestId
                && answer.UserId == value.UserId
            )
            .FirstOrDefaultAsync();
}
