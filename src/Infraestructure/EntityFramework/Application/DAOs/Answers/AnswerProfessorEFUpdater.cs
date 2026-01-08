using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

public sealed class AnswerProfessorEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<Answer, AnswerDomain> domainMapper,
    IUpdateMapper<AnswerUpdateProfessorDTO, Answer> updateMapper
) : EFUpdater<AnswerDomain, AnswerUpdateProfessorDTO, Answer>(ctx, domainMapper, updateMapper)
{
    protected override Task<Answer?> GetTrackedByDTO(AnswerUpdateProfessorDTO value) =>
        _dbSet
            .Where(answer =>
                answer.ClassId == value.ClassId
                && answer.TestId == value.TestId
                && answer.UserId == value.UserId
            )
            .FirstOrDefaultAsync();
}
