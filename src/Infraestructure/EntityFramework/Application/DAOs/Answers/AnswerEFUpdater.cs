using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

public class AnswerEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<Answer, AnswerDomain> domainMapper,
    IUpdateMapper<AnswerUpdateDTO, Answer> updateMapper
) : EFUpdater<AnswerDomain, AnswerUpdateDTO, Answer>(ctx, domainMapper, updateMapper)
{
    protected override Task<Answer?> GetTrackedByDTO(AnswerUpdateDTO value) => 
        _dbSet
            .Where(a => a.UserId == value.UserId)
            .Where(a => a.ClassId == value.ClassId)
            .Where(a => a.TestId == value.TestId)
            .FirstOrDefaultAsync();
}
