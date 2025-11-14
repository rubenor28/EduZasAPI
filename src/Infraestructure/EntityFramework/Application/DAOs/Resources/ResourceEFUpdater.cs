using Application.DTOs.Resources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Resources;

public sealed class ResourceEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<Resource, ResourceDomain> domainMapper,
    IUpdateMapper<ResourceUpdateDTO, Resource> updateMapper
)
    : SimpleKeyEFUpdater<ulong, ResourceDomain, ResourceUpdateDTO, Resource>(
        ctx,
        domainMapper,
        updateMapper
    );
