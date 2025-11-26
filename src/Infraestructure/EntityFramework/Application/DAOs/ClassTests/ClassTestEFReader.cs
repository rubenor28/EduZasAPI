using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassTests;

public sealed class ClassTestEFReader(
    EduZasDotnetContext ctx,
    IEFProjector<TestPerClass, ClassTestDomain> projector
) : EFReader<ClassTestIdDTO, ClassTestDomain, TestPerClass>(ctx, projector)
{
    protected override Expression<Func<TestPerClass, bool>> GetIdPredicate(ClassTestIdDTO id) =>
        ct => ct.ClassId == id.ClassId && ct.TestId == id.TestId;
}
