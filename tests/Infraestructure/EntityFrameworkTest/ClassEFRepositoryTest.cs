using Application.DTOs.Classes;
using Application.DTOs.Common;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest;

public class ClassEFRepositoryTest : IDisposable
{
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly ClassEFCreator _creator;
    private readonly ClassEFUpdater _updater;
    private readonly ClassEFReader _reader;
    private readonly ClassEFQuerier _querier;
    private readonly ClassEFDeleter _deleter;

    public ClassEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var classMapper = new ClassProjector();

        _querier = new(_ctx, classMapper, 10);
        _reader = new(_ctx, classMapper);
        _creator = new(_ctx, classMapper, new NewClassEFMapper());
        _updater = new(_ctx, classMapper, new UpdateClassEFMapper());
        _deleter = new(_ctx, classMapper);
    }

    [Fact]
    public async Task AddClass_ReturnsClass()
    {
        var newClass = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("A"),
            Subject = Optional.Some("Math"),
            OwnerId = 1,
            Professors = [],
            Executor = new() { Id = 1, Role = UserType.ADMIN },
        };

        var created = await _creator.AddAsync(newClass);

        Assert.NotNull(created);
        Assert.Equal(newClass.Id, created.Id);
        Assert.Equal(newClass.ClassName, created.ClassName);
    }

    [Fact]
    public async Task AddClass_WithDuplicateId_ThrowsDbUpdateException()
    {
        var newClass = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("A"),
            Subject = Optional.Some("Math"),
            OwnerId = 1,
            Professors = [],
            Executor = new() { Id = 1, Role = UserType.ADMIN },
        };
        await _creator.AddAsync(newClass);

        var duplicateClass = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Another Class",
            Color = "#000000",
            Section = Optional.Some("B"),
            Subject = Optional.Some("Science"),
            OwnerId = 2,
            Professors = [],
            Executor = new() { Id = 1, Role = UserType.ADMIN },
        };

        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(duplicateClass));
    }

    [Fact]
    public async Task UpdateClass_ReturnsUpdatedClass()
    {
        var newClass = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("A"),
            Subject = Optional.Some("Math"),
            OwnerId = 1,
            Professors = [],
            Executor = new() { Id = 1, Role = UserType.ADMIN },
        };
        var created = await _creator.AddAsync(newClass);

        var update = new ClassUpdateDTO
        {
            Id = created.Id,
            ClassName = "Updated Class Name",
            Color = "#123456",
            Active = false,
            Section = Optional.Some("B"),
            Subject = Optional.Some("Science"),
            Executor = new() { Id = 1, Role = UserType.ADMIN },
        };

        var updatedClass = await _updater.UpdateAsync(update);

        Assert.NotNull(updatedClass);
        Assert.Equal(update.Id, updatedClass.Id);
        Assert.Equal(update.ClassName, updatedClass.ClassName);
        Assert.Equal(update.Color, updatedClass.Color);
        Assert.Equal(update.Active, updatedClass.Active);
        Assert.Equal(update.Section.Unwrap(), updatedClass.Section.Unwrap());
        Assert.Equal(update.Subject.Unwrap(), updatedClass.Subject.Unwrap());
    }

    [Fact]
    public async Task GetAsync_WhenClassExists_ReturnsClass()
    {
        var newClass = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("A"),
            Subject = Optional.Some("Math"),
            OwnerId = 1,
            Professors = [],
            Executor = new() { Id = 1, Role = UserType.ADMIN },
        };
        var created = await _creator.AddAsync(newClass);

        var foundClass = await _reader.GetAsync(created.Id);

        Assert.True(foundClass.IsSome);
        Assert.Equal(created.Id, foundClass.Unwrap().Id);
    }

    [Fact]
    public async Task GetAsync_WhenClassDoesNotExist_ReturnsEmptyOptional()
    {
        var foundClass = await _reader.GetAsync("non-existent-class");

        Assert.True(foundClass.IsNone);
    }

    [Fact]
    public async Task DeleteAsync_WhenClassExists_ReturnsDeletedClass()
    {
        var newClass = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("A"),
            Subject = Optional.Some("Math"),
            OwnerId = 1,
            Professors = [],
            Executor = new() { Id = 1, Role = UserType.ADMIN },
        };
        var created = await _creator.AddAsync(newClass);

        var deletedClass = await _deleter.DeleteAsync(created.Id);

        Assert.NotNull(deletedClass);
        Assert.Equal(created.Id, deletedClass.Id);

        var foundClass = await _reader.GetAsync(created.Id);
        Assert.True(foundClass.IsNone);
    }

    [Fact]
    public async Task DeleteAsync_WhenClassDoesNotExist_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _deleter.DeleteAsync("non-existent-class")
        );
    }

    [Fact]
    public async Task GetByAsync_WithClassNameCriteria_ReturnsMatchingClass()
    {
        var newClass1 = new NewClassDTO
        {
            Id = "class1",
            ClassName = "Math Class",
            Color = "#ffffff",
            Section = Optional.Some("A"),
            Subject = Optional.Some("Math"),
            OwnerId = 1,
            Professors = [],
            Executor = new() { Id = 1, Role = UserType.ADMIN },
        };
        await _creator.AddAsync(newClass1);

        var newClass2 = new NewClassDTO
        {
            Id = "class2",
            ClassName = "Science Class",
            Color = "#ffffff",
            Section = Optional.Some("B"),
            Subject = Optional.Some("Science"),
            OwnerId = 2,
            Professors = [],
            Executor = new() { Id = 1, Role = UserType.ADMIN },
        };
        await _creator.AddAsync(newClass2);

        var criteria = new ClassCriteriaDTO
        {
            ClassName = new StringQueryDTO
            {
                Text = "Math Class",
                SearchType = StringSearchType.EQ,
            },
        };

        var result = await _querier.GetByAsync(criteria);

        Assert.Single(result.Results);
        Assert.Equal(newClass1.ClassName, result.Results.First().ClassName);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
