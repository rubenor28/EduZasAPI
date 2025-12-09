using Application.DTOs.Classes;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Classes;

/// <summary>
/// Implementación de creación de clases usando EF.
/// </summary>
public class ClassEFCreator(
    EduZasDotnetContext ctx,
    IMapper<Class, ClassDomain> domainMapper,
    IMapper<NewClassDTO, Class> newEntityMapper
) : EFCreator<ClassDomain, NewClassDTO, Class>(ctx, domainMapper, newEntityMapper);
