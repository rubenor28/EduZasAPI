using Application.DTOs.ClassTests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassTests;

public sealed class ClassTestEFCreator(
    EduZasDotnetContext ctx,
    IMapper<TestPerClass, ClassTestDomain> domainMapper,
    IMapper<NewClassTestDTO, TestPerClass> newEntityMapper
) : EFCreator<ClassTestDomain, NewClassTestDTO, TestPerClass>(ctx, domainMapper, newEntityMapper);
