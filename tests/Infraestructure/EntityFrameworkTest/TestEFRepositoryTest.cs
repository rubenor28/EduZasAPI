using Application.DTOs.Common;
using Application.DTOs.Tests;
using Domain.Enums;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Tests;
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

    public TestEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testMapper = new TestMapper();
        var testProjector = new TestProjector();

        _creator = new(_ctx, testMapper, new NewTestEFMapper());
        _updater = new(_ctx, testMapper, new UpdateTestEFMapper());
        _reader = new(_ctx, testMapper);
        _querier = new(_ctx, testProjector, 10);
        _deleter = new(_ctx, testMapper);
    }
    
    private async Task<User> CreateProfessor(ulong id = 1)
    {
        var professor = new User
        {
            UserId = id,
            Email = $"professor{id}@example.com",
            FirstName = "Test",
            FatherLastname = "Professor",
            Password = "hashedpassword", 
            Role = (uint)UserType.PROFESSOR,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };
        _ctx.Users.Add(professor);
        await _ctx.SaveChangesAsync();
        _ctx.ChangeTracker.Clear();
        return professor;
    }


    [Fact]
    public async Task AddTest_ReturnsTest()
    {
        var professor = await CreateProfessor();
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = professor.UserId,
        };

        var created = await _creator.AddAsync(newTest);

        Assert.NotNull(created);
        Assert.Equal(newTest.Title, created.Title);
        Assert.Equal(newTest.Content, created.Content.GetValue<string>());
    }

    [Fact]
    public async Task UpdateTest_ReturnsUpdatedTest()
    {
        var professor = await CreateProfessor();
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = professor.UserId,
        };

        var created = await _creator.AddAsync(newTest);
        _ctx.ChangeTracker.Clear();

        var update = new TestUpdateDTO
        {
            Id = created.Id,
            Title = "Updated Test Title",
            Content = "Updated Test Content",
            ProfessorId = professor.UserId,
            Active = false
        };

        var updatedTest = await _updater.UpdateAsync(update);

        Assert.NotNull(updatedTest);
        Assert.Equal(update.Title, updatedTest.Title);
        Assert.Equal(update.Active, updatedTest.Active);
        Assert.Equal(update.Content, updatedTest.Content.GetValue<string>());
    }

    [Fact]
    public async Task GetAsync_WhenTestExists_ReturnsTest()
    {
        var professor = await CreateProfessor();
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = professor.UserId,
        };
        var created = await _creator.AddAsync(newTest);

        var foundTest = await _reader.GetAsync(created.Id);

        Assert.NotNull(foundTest);
        Assert.Equal(created.Id, foundTest.Id);
    }

    [Fact]
    public async Task GetAsync_WhenTestDoesNotExist_ReturnsEmptyOptional()
    {
        var foundTest = await _reader.GetAsync(Guid.NewGuid());
        Assert.Null(foundTest);
    }

    [Fact]
    public async Task DeleteAsync_WhenTestExists_ReturnsDeletedTest()
    {
        var professor = await CreateProfessor();
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = professor.UserId,
        };
        var created = await _creator.AddAsync(newTest);
        _ctx.ChangeTracker.Clear();

        var deletedTest = await _deleter.DeleteAsync(created.Id);

        Assert.NotNull(deletedTest);
        Assert.Equal(created.Id, deletedTest.Id);

        var foundTest = await _reader.GetAsync(created.Id);
        Assert.Null(foundTest);
    }

    [Fact]
    public async Task DeleteAsync_WhenTestDoesNotExist_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _deleter.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByAsync_WithTitleCriteria_ReturnsMatchingTest()
    {
        var professor = await CreateProfessor();
        var newTest1 = new NewTestDTO
        {
            Title = "Math Test",
            Content = "Test Content",
            ProfessorId = professor.UserId,
        };
        await _creator.AddAsync(newTest1);

        var newTest2 = new NewTestDTO
        {
            Title = "Science Test",
            Content = "Test Content",
            ProfessorId = professor.UserId,
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
