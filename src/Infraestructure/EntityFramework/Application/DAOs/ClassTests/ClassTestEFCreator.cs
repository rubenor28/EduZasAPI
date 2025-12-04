using Application.DTOs.ClassTests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassTests;

public sealed class ClassTestEFCreator(
    EduZasDotnetContext ctx,
    IMapper<TestPerClass, ClassTestDomain> domainMapper,
    IMapper<ClassTestDTO, TestPerClass> newEntityMapper
) : EFCreator<ClassTestDomain, ClassTestDTO, TestPerClass>(ctx, domainMapper, newEntityMapper);
