using Application.DTOs.ClassResources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassResources;

public sealed class ClassResourceEFDeleter(
    EduZasDotnetContext ctx,
    IMapper<ClassResource, ClassResourceDomain> domainMapper
) : EFDeleter<ClassResourceIdDTO, ClassResourceDomain, ClassResource>(ctx, domainMapper)
{
    public override async Task<ClassResource?> GetTrackedById(ClassResourceIdDTO id) =>
        await _dbSet
            .AsTracking()
            .Where(r => r.ResourceId == id.ResourceId && r.ClassId == id.ClassId)
            .FirstOrDefaultAsync();
}
