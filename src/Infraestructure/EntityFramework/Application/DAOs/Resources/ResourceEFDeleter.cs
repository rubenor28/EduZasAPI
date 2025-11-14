using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Resources;

public sealed class ResourceEFDeleter(
    EduZasDotnetContext ctx,
    IMapper<Resource, ResourceDomain> domainMapper
) : SimpleKeyEFDeleter<ulong, ResourceDomain, Resource>(ctx, domainMapper);
