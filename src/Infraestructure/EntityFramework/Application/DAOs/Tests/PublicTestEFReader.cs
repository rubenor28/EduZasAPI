using Application.DAOs;
using Application.DTOs.Tests;
using Domain.Entities.PublicQuestions;
using Domain.Entities.Questions;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

public class PublicTestEFReader(
    EduZasDotnetContext ctx,
    IMapper<Guid, IQuestion, IPublicQuestion> questionMapper
)
    : EntityFrameworkDAO<PublicTestDTO, TestPerClass>(ctx),
        IReaderAsync<PublicTestIdDTO, PublicTestDTO>
{
    private readonly IMapper<Guid, IQuestion, IPublicQuestion> _questionMapper = questionMapper;

    public async Task<PublicTestDTO?> GetAsync(PublicTestIdDTO id)
    {
        var tpc = await _dbSet
            .AsQueryable()
            .AsNoTracking()
            .Include(tpc => tpc.Test)
            .Where(tpc => tpc.TestId == id.TestId && tpc.ClassId == id.ClassId)
            .FirstOrDefaultAsync();

        if (tpc is null)
            return null;

        return new PublicTestDTO
        {
            ClassId = tpc.ClassId,
            Active = tpc.Test.Active,
            Color = tpc.Test.Color,
            Content = tpc.Test.Content.Select(q => _questionMapper.Map(q.Key, q.Value)),
            Id = tpc.Test.TestId,
            ProfessorId = tpc.Test.ProfessorId,
            Title = tpc.Test.Title,
            Deadline = tpc.Test.TimeLimitMinutes.HasValue
                ? tpc.CreatedAt.ToUniversalTime()
                    .Add(TimeSpan.FromMinutes((double)tpc.Test.TimeLimitMinutes.Value))
                : null,
        };
    }
}
