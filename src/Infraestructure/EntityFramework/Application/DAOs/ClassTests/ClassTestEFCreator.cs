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
    IMapper<ClassTestIdDTO, TestPerClass> newEntityMapper
) : EFCreator<ClassTestDomain, ClassTestIdDTO, TestPerClass>(ctx, domainMapper, newEntityMapper);

