using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Tests;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Tests;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Tests;

class MockUintValidator : IBusinessValidationService<ulong>
{
    public Result<Unit, IEnumerable<FieldErrorDTO>> IsValid(ulong data)
    {
        if (data <= 0)
            return new FieldErrorDTO[]
            {
                new() { Field = "id", Message = "Debe ser un numero entero" },
            };

        return Unit.Value;
    }
}

public class ReadTestUseCaseTest : IDisposable
{
    private readonly ReadTestUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly TestProjector _testMapper = new();
    private readonly UserProjector _userMapper = new();

    private readonly Random _random = new();

    public ReadTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testMapper = new TestProjector();
        var testReader = new TestEFReader(_ctx, testMapper);
        var validator = new MockUintValidator();

        _useCase = new ReadTestUseCase(testReader, validator);
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.PROFESSOR)
    {
        var id = (ulong)_random.Next(1000, 100000);
        var user = new User
        {
            UserId = id,
            Email = $"user{id}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)role,
        };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return _userMapper.Map(user);
    }

    private async Task<TestDomain> SeedTest(ulong professorId)
    {
        var test = new Test
        {
            Title = "Original Title",
            Content = "Original Description",
            ProfessorId = professorId,
        };
        _ctx.Tests.Add(test);
        await _ctx.SaveChangesAsync();
        return _testMapper.Map(test);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidId_ReturnsOk()
    {
        var professor = await SeedUser();
        var test = await SeedTest(professor.Id);

        var result = await _useCase.ExecuteAsync(test.Id);

        Assert.True(result.IsOk);
        var foundTest = result.Unwrap();
        Assert.NotNull(foundTest);
        Assert.Equal(test.Id, foundTest.Id);
    }

    [Fact]
    public async Task ExecuteAsync_TestNotFound_ReturnsError()
    {
        var result = await _useCase.ExecuteAsync(999);

        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
