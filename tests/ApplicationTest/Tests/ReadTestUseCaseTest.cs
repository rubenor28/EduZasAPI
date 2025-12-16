using Application.DTOs.Common;
using Application.UseCases.Tests;
using Domain.Entities;
using Domain.Entities.Questions;
using Domain.Enums;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Tests;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Tests;

public class ReadTestUseCaseTest : IDisposable
{
    private readonly ReadTestUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly TestMapper _testMapper = new();
    private readonly UserMapper _userMapper = new();

    private readonly Random _random = new();

    public ReadTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testMapper = new TestMapper();
        var testReader = new TestEFReader(_ctx, testMapper);

        _useCase = new ReadTestUseCase(testReader);
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
            Content = new Dictionary<Guid, IQuestion>(),
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

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = test.Id,
                Executor = new() { Id = professor.Id, Role = professor.Role },
            }
        );

        Assert.True(result.IsOk);
        var foundTest = result.Unwrap();
        Assert.NotNull(foundTest);
        Assert.Equal(test.Id, foundTest.Id);
    }

    [Fact]
    public async Task ExecuteAsync_TestNotFound_ReturnsError()
    {
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = Guid.NewGuid(),
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

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
