using Application.DTOs.ClassTests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassTests;

/// <summary>
/// Implementación de creación de relaciones Clase-Examen usando EF.
/// </summary>
public sealed class ClassTestEFCreator(
    EduZasDotnetContext ctx,
    IMapper<TestPerClass, ClassTestDomain> domainMapper,
    IMapper<ClassTestDTO, TestPerClass> newEntityMapper
) : EFCreator<ClassTestDomain, ClassTestDTO, TestPerClass>(ctx, domainMapper, newEntityMapper);

