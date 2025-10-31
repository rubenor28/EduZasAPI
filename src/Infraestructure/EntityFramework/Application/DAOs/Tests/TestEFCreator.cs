using Application.DTOs.Tests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tests;

public class TestEFCreator(
    EduZasDotnetContext ctx,
    IMapper<Test, TestDomain> domainMapper,
    IMapper<NewTestDTO, Test> newEntityMapper
) : EFCreator<TestDomain, NewTestDTO, Test>(ctx, domainMapper, newEntityMapper);