using System.Linq.Expressions;
using Application.DTOs.ClassResources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassResources;

public sealed class ClassResourceEFReader(
    EduZasDotnetContext ctx,
    IMapper<ClassResource, ClassResourceDomain> mapper
) : EFReader<ClassResourceIdDTO, ClassResourceDomain, ClassResource>(ctx, mapper)
{
    protected override Expression<Func<ClassResource, bool>> GetIdPredicate(
        ClassResourceIdDTO id
    ) => cr => cr.ClassId == id.ClassId && cr.ResourceId == id.ResourceId;
}
