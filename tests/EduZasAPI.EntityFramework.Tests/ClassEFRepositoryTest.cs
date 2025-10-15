
using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Classes;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Classes;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Tests.EntityFramework;

public class ClassEFRepositoryTest : IDisposable
{
    private readonly IRepositoryAsync<string, ClassDomain, NewClassDTO, ClassUpdateDTO, DeleteClassDTO, ClassCriteriaDTO> _repository;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public ClassEFRepositoryTest()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>()
          .UseSqlite(_conn)
          .Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();
        _repository = new ClassEntityFrameworkRepository(_ctx, 10);
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
            OwnerId = 1
        };

        var created = await _repository.AddAsync(newClass);

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
            OwnerId = 1
        };
        await _repository.AddAsync(newClass);

        var duplicateClass = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Another Class",
            Color = "#000000",
            Section = Optional.Some("B"),
            Subject = Optional.Some("Science"),
            OwnerId = 2
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.AddAsync(duplicateClass));
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
            OwnerId = 1
        };
        var created = await _repository.AddAsync(newClass);

        var update = new ClassUpdateDTO
        {
            Id = created.Id,
            ClassName = "Updated Class Name",
            Color = "#123456",
            Active = false,
            Section = Optional.Some("B"),
            Subject = Optional.Some("Science"),
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR }
        };

        var updatedClass = await _repository.UpdateAsync(update);

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
            OwnerId = 1
        };
        var created = await _repository.AddAsync(newClass);

        var foundClass = await _repository.GetAsync(created.Id);

        Assert.True(foundClass.IsSome);
        Assert.Equal(created.Id, foundClass.Unwrap().Id);
    }

    [Fact]
    public async Task GetAsync_WhenClassDoesNotExist_ReturnsEmptyOptional()
    {
        var foundClass = await _repository.GetAsync("non-existent-class");

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
            OwnerId = 1
        };
        var created = await _repository.AddAsync(newClass);

        var deletedClass = await _repository.DeleteAsync(created.Id);

        Assert.NotNull(deletedClass);
        Assert.Equal(created.Id, deletedClass.Id);

        var foundClass = await _repository.GetAsync(created.Id);
        Assert.True(foundClass.IsNone);
    }

    [Fact]
    public async Task DeleteAsync_WhenClassDoesNotExist_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.DeleteAsync("non-existent-class"));
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
            OwnerId = 1
        };
        await _repository.AddAsync(newClass1);

        var newClass2 = new NewClassDTO
        {
            Id = "class2",
            ClassName = "Science Class",
            Color = "#ffffff",
            Section = Optional.Some("B"),
            Subject = Optional.Some("Science"),
            OwnerId = 2
        };
        await _repository.AddAsync(newClass2);

        var criteria = new ClassCriteriaDTO
        {
            ClassName = Optional.Some(new StringQueryDTO { Text = "Math Class", SearchType = StringSearchType.EQ })
        };

        var result = await _repository.GetByAsync(criteria);

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
