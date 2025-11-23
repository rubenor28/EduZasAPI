using Application.DTOs.Common;
using Application.DTOs.Tests;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest;

public class TestEFRepositoryTest : IDisposable
{
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly TestEFCreator _creator;
    private readonly TestEFUpdater _updater;
    private readonly TestEFReader _reader;
    private readonly TestEFQuerier _querier;
    private readonly TestEFDeleter _deleter;

    private readonly UserEFMapper _userMapper;

    public TestEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testMapper = new TestEFMapper();

        _creator = new(_ctx, testMapper, testMapper);
        _updater = new(_ctx, testMapper, testMapper);
        _reader = new(_ctx, testMapper);
        _querier = new(_ctx, testMapper, 10);
        _deleter = new(_ctx, testMapper);

        _userMapper = new UserEFMapper(new UserTypeUintMapper());
    }

    private async Task<UserDomain> CreateUser(UserType role = UserType.STUDENT)
    {
        var user = new User
        {
            Email = "test@test.com",
            Password = "test",
            FirstName = "test",
            FatherLastname = "test",
            Active = true,
            Role = (uint)role,
        };

        await _ctx.Users.AddAsync(user);
        await _ctx.SaveChangesAsync();

        return _userMapper.Map(user);
    }

    [Fact]
    public async Task AddTest_ReturnsTest()
    {
        var user = await CreateUser(UserType.PROFESSOR);

        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = 1,
            Executor = new() { Id = user.Id, Role = user.Role },
        };

        var created = await _creator.AddAsync(newTest);

        Assert.NotNull(created);
        Assert.Equal(newTest.Title, created.Title);
        Assert.Equal(newTest.Content, created.Content);
    }

    [Fact]
    public async Task UpdateTest_ReturnsUpdatedTest()
    {
        var user = await CreateUser(UserType.PROFESSOR);

        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = 1,
            Executor = new() { Id = user.Id, Role = user.Role },
        };

        var created = await _creator.AddAsync(newTest);

        var update = new TestUpdateDTO
        {
            Id = created.Id,
            Title = "Updated Test Title",
            Content = "Updated Test Content",
            ProfessorId = 1,
            Executor = new() { Id = user.Id, Role = user.Role },
        };

        var updatedTest = await _updater.UpdateAsync(update);

        Assert.NotNull(updatedTest);
        Assert.Equal(update.Title, updatedTest.Title);
        Assert.Equal(update.Content, updatedTest.Content);
    }

    [Fact]
    public async Task GetAsync_WhenTestExists_ReturnsTest()
    {
        var user = await CreateUser(UserType.PROFESSOR);

        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = 1,
            Executor = new() { Id = user.Id, Role = user.Role },
        };
        var created = await _creator.AddAsync(newTest);

        var foundTest = await _reader.GetAsync(created.Id);

        Assert.True(foundTest.IsSome);
        Assert.Equal(created.Id, foundTest.Unwrap().Id);
    }

    [Fact]
    public async Task GetAsync_WhenTestDoesNotExist_ReturnsEmptyOptional()
    {
        var foundTest = await _reader.GetAsync(999);

        Assert.True(foundTest.IsNone);
    }

    [Fact]
    public async Task DeleteAsync_WhenTestExists_ReturnsDeletedTest()
    {
        var user = await CreateUser(UserType.PROFESSOR);
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = 1,
            Executor = new() { Id = user.Id, Role = user.Role },
        };
        var created = await _creator.AddAsync(newTest);

        var deletedTest = await _deleter.DeleteAsync(created.Id);

        Assert.NotNull(deletedTest);
        Assert.Equal(created.Id, deletedTest.Id);

        var foundTest = await _reader.GetAsync(created.Id);
        Assert.True(foundTest.IsNone);
    }

    [Fact]
    public async Task DeleteAsync_WhenTestDoesNotExist_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _deleter.DeleteAsync(999));
    }

    [Fact]
    public async Task GetByAsync_WithTitleCriteria_ReturnsMatchingTest()
    {
        var user = await CreateUser(UserType.PROFESSOR);
        var newTest1 = new NewTestDTO
        {
            Title = "Math Test",
            Content = "Test Content",
            ProfessorId = 1,
            Executor = new() { Id = user.Id, Role = user.Role },
        };
        await _creator.AddAsync(newTest1);

        var newTest2 = new NewTestDTO
        {
            Title = "Science Test",
            Content = "Test Content",
            ProfessorId = 1,
            Executor = new() { Id = user.Id, Role = user.Role },
        };
        await _creator.AddAsync(newTest2);

        var criteria = new TestCriteriaDTO
        {
            Title = new StringQueryDTO { Text = "Math Test", SearchType = StringSearchType.EQ },
        };

        var result = await _querier.GetByAsync(criteria);

        Assert.Single(result.Results);
        Assert.Equal(newTest1.Title, result.Results.First().Title);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
