using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tests;

public sealed class TestEFReader(EduZasDotnetContext ctx, IMapper<Test, TestDomain> domainMapper)
    : SimpleKeyEFReader<ulong, TestDomain, Test>(ctx, domainMapper);
