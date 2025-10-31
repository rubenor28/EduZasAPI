using Application.DTOs.Tests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tests;

public sealed class TestEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<Test, TestDomain> domainMapper,
    IUpdateMapper<TestUpdateDTO, Test> updateMapper
) : SimpleKeyEFUpdater<ulong, TestDomain, TestUpdateDTO, Test>(ctx, domainMapper, updateMapper);
