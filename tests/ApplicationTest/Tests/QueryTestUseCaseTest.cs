using Application.DTOs.Common;
using Application.DTOs.Tests;
using Application.UseCases.Tests;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Tests;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Tests;

public class QueryTestUseCaseTest : IDisposable
{
    private readonly QueryTestUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly UserMapper _userMapper = new();

    private readonly Random _random = new();

    public QueryTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testProjector = new TestProjector();
        var testQuerier = new TestEFQuerier(_ctx, testProjector, 10);

        _useCase = new QueryTestUseCase(testQuerier);
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

    private static Executor AsExecutor(UserDomain u) => new() { Id = u.Id, Role = u.Role };

    private async Task<(UserDomain, UserDomain)> SeedTests()
    {
        var professor1 = await SeedUser();
        var professor2 = await SeedUser();

        _ctx.Tests.AddRange(
            new Test
            {
                TestId = Guid.NewGuid(),
                Title = "Math Test",
                Content = "Test for algebra",
                ProfessorId = professor1.Id,
            },
            new Test
            {
                TestId = Guid.NewGuid(),
                Title = "Science Test",
                Content = "Test for biology",
                ProfessorId = professor1.Id,
            },
            new Test
            {
                TestId = Guid.NewGuid(),
                Title = "History Test",
                Content = "Test for world history",
                ProfessorId = professor2.Id,
            }
        );
        await _ctx.SaveChangesAsync();
        return (professor1, professor2);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoCriteria_ReturnsAllTests()
    {
        var admin = await SeedUser(UserType.ADMIN);
        await SeedTests();
        var criteria = new TestCriteriaDTO();

        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        Assert.Equal(3, search.Results.Count());
    }

    [Fact]
    public async Task ExecuteAsync_WithTitleCriteria_ReturnsMatchingTests()
    {
        var admin = await SeedUser(UserType.ADMIN);
        await SeedTests();
        var criteria = new TestCriteriaDTO
        {
            Title = new StringQueryDTO { Text = "Math", SearchType = StringSearchType.LIKE },
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        var results = search.Results;
        Assert.Single(results);
        Assert.Equal("Math Test", results.First().Title);
    }

    [Fact]
    public async Task ExecuteAsync_WithProfessorIdCriteria_ReturnsMatchingTests()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var (_, professor2) = await SeedTests();
        var criteria = new TestCriteriaDTO { ProfessorId = professor2.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        var results = search.Results;
        Assert.Single(results);
        Assert.Equal(professor2.Id, results.First().ProfessorId);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
