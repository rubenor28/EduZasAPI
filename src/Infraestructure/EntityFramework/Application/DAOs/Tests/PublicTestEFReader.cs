using System.Linq.Expressions;
using Application.DTOs.Tests;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

public class PublicTestEFReader(EduZasDotnetContext ctx, IMapper<Test, PublicTestDTO> mapper)
    : EFReader<PublicTestIdDTO, PublicTestDTO, Test>(ctx, mapper)
{
    protected override Expression<Func<Test, bool>> GetIdPredicate(PublicTestIdDTO value) =>
        t => t.TestId == value.TestId;
}
