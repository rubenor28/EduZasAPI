using System.Linq.Expressions;
using Application.DTOs.ClassTests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassTests;

public sealed class ClassTestEFReader(
    EduZasDotnetContext ctx,
    IMapper<TestPerClass, ClassTestDomain> mapper
) : EFReader<ClassTestIdDTO, ClassTestDomain, TestPerClass>(ctx, mapper)
{
    protected override Expression<Func<TestPerClass, bool>> GetIdPredicate(ClassTestIdDTO id) =>
        ct => ct.ClassId == id.ClassId && ct.TestId == id.TestId;
}
