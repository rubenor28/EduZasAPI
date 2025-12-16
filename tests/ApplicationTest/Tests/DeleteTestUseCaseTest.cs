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

public class DeleteTestUseCaseTest : IDisposable
{
    private readonly DeleteTestUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly TestMapper _testMapper = new();
    private readonly UserMapper _userMapper = new();

    private readonly Random _random = new();

    public DeleteTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testDeleter = new TestEFDeleter(_ctx, _testMapper);
        var testReader = new TestEFReader(_ctx, _testMapper);

        _useCase = new DeleteTestUseCase(testDeleter, testReader);
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
            TestId = Guid.NewGuid(),
            Title = "Original Title",
            Content = new Dictionary<Guid, IQuestion>(),
            ProfessorId = professorId,
        };
        _ctx.Tests.Add(test);
        await _ctx.SaveChangesAsync();
        return _testMapper.Map(test);
    }

    private static Executor AsExecutor(UserDomain user) => new() { Id = user.Id, Role = user.Role };

    [Fact]
    public async Task ExecuteAsync_WithValidId_ReturnsOk()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var seeded = await SeedTest(admin.Id);

        var result = await _useCase.ExecuteAsync(
            new() { Data = seeded.Id, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_TestNotFound_ReturnsError()
    {
        var user = await SeedUser(UserType.ADMIN);

        var result = await _useCase.ExecuteAsync(
            new() { Data = Guid.NewGuid(), Executor = AsExecutor(user) }
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
