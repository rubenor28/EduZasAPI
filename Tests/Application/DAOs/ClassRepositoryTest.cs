using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

public class ClassRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<ClassDomain, NewClassDTO> _creator;
    private readonly IUpdaterAsync<ClassDomain, ClassUpdateDTO> _updater;
    private readonly IReaderAsync<string, ClassDomain> _reader;
    private readonly IQuerierAsync<ClassDomain, ClassCriteriaDTO> _querier;
    private readonly IDeleterAsync<string, ClassDomain> _deleter;

    public ClassRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<ClassDomain, NewClassDTO>>();
        _updater = _sp.GetRequiredService<IUpdaterAsync<ClassDomain, ClassUpdateDTO>>();
        _reader = _sp.GetRequiredService<IReaderAsync<string, ClassDomain>>();
        _querier = _sp.GetRequiredService<IQuerierAsync<ClassDomain, ClassCriteriaDTO>>();
        _deleter = _sp.GetRequiredService<IDeleterAsync<string, ClassDomain>>();
    }

    [Fact]
    public async Task AddClass_ReturnsClass()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = "A",
            Subject = "Math",
            OwnerId = owner.Id,
            Professors = [],
        };

        // Act
        var created = await _creator.AddAsync(newClass);

        // Assert
        Assert.NotNull(created);
        Assert.Equal(newClass.Id, created.Id);
        Assert.Equal(newClass.ClassName, created.ClassName);
    }

    [Fact]
    public async Task AddClass_WithDuplicateId_ThrowsDbUpdateException()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = "A",
            Subject = "Math",
            OwnerId = owner.Id,
            Professors = [],
        };
        await _creator.AddAsync(newClass);

        var duplicateClass = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Another Class",
            Color = "#000000",
            Section = "B",
            Subject = "Science",
            OwnerId = owner.Id,
            Professors = [],
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(duplicateClass));
    }

    [Fact]
    public async Task UpdateClass_ReturnsUpdatedClass()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var created = await SeedClass(owner.Id, "test-class");

        var update = new ClassUpdateDTO
        {
            Id = created.Id,
            ClassName = "Updated Class Name",
            Color = "#123456",
            Active = false,
            Section = "B",
            Subject = "Science",
        };

        // Act
        var updatedClass = await _updater.UpdateAsync(update);

        // Assert
        Assert.NotNull(updatedClass);
        Assert.Equal(update.Id, updatedClass.Id);
        Assert.Equal(update.ClassName, updatedClass.ClassName);
        Assert.Equal(update.Color, updatedClass.Color);
        Assert.Equal(update.Active, updatedClass.Active);
        Assert.Equal(update.Section, updatedClass.Section);
        Assert.Equal(update.Subject, updatedClass.Subject);
    }

    [Fact]
    public async Task GetAsync_WhenClassExists_ReturnsClass()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var created = await SeedClass(owner.Id, "test-class");

        // Act
        var foundClass = await _reader.GetAsync(created.Id);

        // Assert
        Assert.NotNull(foundClass);
        Assert.Equal(created.Id, foundClass.Id);
    }

    [Fact]
    public async Task GetAsync_WhenClassDoesNotExist_ReturnsEmptyOptional()
    {
        // Act
        var foundClass = await _reader.GetAsync("non-existent-class");

        // Assert
        Assert.Null(foundClass);
    }

    [Fact]
    public async Task DeleteAsync_WhenClassExists_ReturnsDeletedClass()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var created = await SeedClass(owner.Id, "test-class");

        // Act
        var deletedClass = await _deleter.DeleteAsync(created.Id);

        // Assert
        Assert.NotNull(deletedClass);
        Assert.Equal(created.Id, deletedClass.Id);

        var foundClass = await _reader.GetAsync(created.Id);
        Assert.Null(foundClass);
    }

    [Fact]
    public async Task DeleteAsync_WhenClassDoesNotExist_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _deleter.DeleteAsync("non-existent-class")
        );
    }

    [Fact]
    public async Task GetByAsync_WithClassNameCriteria_ReturnsMatchingClass()
    {
        // Arrange
        var owner1 = await SeedUser(UserType.PROFESSOR);
        var owner2 = await SeedUser(UserType.PROFESSOR);
        await SeedClass(owner1.Id, "class1");
        await SeedClass(owner2.Id, "class2");

        var criteria = new ClassCriteriaDTO
        {
            ClassName = new StringQueryDTO
            {
                Text = "Test Class", // SeedClass creates it with this name
                SearchType = StringSearchType.EQ,
            },
        };

        // Act
        var result = await _querier.GetByAsync(criteria);

        // Assert
        Assert.Equal(2, result.Results.Count());
    }
}

