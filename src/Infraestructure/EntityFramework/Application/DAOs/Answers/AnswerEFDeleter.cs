using Application.DTOs.Answers;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Answers;

public sealed class AnswerEFDeleter(EduZasDotnetContext ctx, IMapper<Answer, AnswerDomain> mapper)
    : EFDeleter<AnswerIdDTO, AnswerDomain, Answer>(ctx, mapper)
{
    public override Task<Answer?> GetTrackedById(AnswerIdDTO id) =>
        _dbSet
            .AsTracking()
            .Where(a => a.UserId == id.UserId && a.TestId == id.TestId && a.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
