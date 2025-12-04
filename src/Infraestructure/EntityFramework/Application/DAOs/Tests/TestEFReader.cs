using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tests;

public sealed class TestEFReader(EduZasDotnetContext ctx, IMapper<Test, TestDomain> mapper)
    : EFReader<Guid, TestDomain, Test>(ctx, mapper)
{
    protected override Expression<Func<Test, bool>> GetIdPredicate(Guid id) => t => t.TestId == id;
}
