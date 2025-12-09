using Application.DTOs.Resources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Resources;

/// <summary>
/// Implementación de creación de recursos usando EF.
/// </summary>
public sealed class ResourceEFCreator(
    EduZasDotnetContext ctx,
    IMapper<Resource, ResourceDomain> domainMapper,
    IMapper<NewResourceDTO, Resource> newEntityMapper
) : EFCreator<ResourceDomain, NewResourceDTO, Resource>(ctx, domainMapper, newEntityMapper);
