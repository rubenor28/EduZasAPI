using Application.DTOs.ResourceViewSessions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ResourceViewSessions;

using INewResourceMapper = IMapper<NewResourceViewSession, ResourceViewSession>;
using IResourceMapper = IMapper<ResourceViewSession, ResourceViewSessionDomain>;

public class ResourceViewSessionEFCreator(
    EduZasDotnetContext ctx,
    IResourceMapper domainMapper,
    INewResourceMapper newEntityMapper
)
    : EFCreator<ResourceViewSessionDomain, NewResourceViewSession, ResourceViewSession>(
        ctx,
        domainMapper,
        newEntityMapper
    );
