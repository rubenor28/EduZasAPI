using Application.DTOs.ClassResources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassResources;

/// <summary>
/// Implementación de creación de relaciones Clase-Recurso usando EF.
/// </summary>
public sealed class ClassResourceEFCreator(
    EduZasDotnetContext ctx,
    IMapper<ClassResource, ClassResourceDomain> domainMapper,
    IMapper<ClassResourceDTO, ClassResource> newEntityMapper
)
    : EFCreator<ClassResourceDomain, ClassResourceDTO, ClassResource>(
        ctx,
        domainMapper,
        newEntityMapper
    );
