using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tests;

public sealed class TestEFReader(EduZasDotnetContext ctx, IEFProjector<Test, TestDomain> projector)
    : EFReader<ulong, TestDomain, Test>(ctx, projector)
{
    protected override Expression<Func<Test, bool>> GetIdPredicate(ulong id) => t => t.TestId == id;
}
