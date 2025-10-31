using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tests;

public sealed class TestEFDeleter(EduZasDotnetContext ctx, IMapper<Test, TestDomain> domainMapper)
    : SimpleKeyEFDeleter<ulong, TestDomain, Test>(ctx, domainMapper);
