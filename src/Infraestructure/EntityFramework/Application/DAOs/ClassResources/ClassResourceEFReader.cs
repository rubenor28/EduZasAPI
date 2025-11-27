using System.Linq.Expressions;
using Application.DTOs.ClassResources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassResources;

public sealed class ClassResourceEFReader(
    EduZasDotnetContext ctx,
    IEFProjector<ClassResource, ClassResourceDomain> projector
) : EFReader<ClassResourceIdDTO, ClassResourceDomain, ClassResource>(ctx, projector)
{
    protected override Expression<Func<ClassResource, bool>> GetIdPredicate(
        ClassResourceIdDTO id
    ) => cr => cr.ClassId == id.ClassId && cr.ResourceId == id.ResourceId;
}
