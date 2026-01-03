using Application.DAOs;
using Application.DTOs.ClassStudents;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

public class ClassStudentsRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<ClassStudentDomain, UserClassRelationId> _creator;
    private readonly IReaderAsync<UserClassRelationId, ClassStudentDomain> _reader;
    private readonly IUpdaterAsync<ClassStudentDomain, ClassStudentUpdateDTO> _updater;
    private readonly IDeleterAsync<UserClassRelationId, ClassStudentDomain> _deleter;

    public ClassStudentsRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<ClassStudentDomain, UserClassRelationId>>();
        _reader = _sp.GetRequiredService<IReaderAsync<UserClassRelationId, ClassStudentDomain>>();
        _updater = _sp.GetRequiredService<IUpdaterAsync<ClassStudentDomain, ClassStudentUpdateDTO>>();
        _deleter = _sp.GetRequiredService<IDeleterAsync<UserClassRelationId, ClassStudentDomain>>();
    }

    [Fact]
    public async Task Add_ValidRelation_ReturnsRelation()
    {
        // Arrange
        var user = await SeedUser();
        var cls = await SeedClass(user.Id);
        var newRelation = new UserClassRelationId { ClassId = cls.Id, UserId = user.Id };

        // Act
        var created = await _creator.AddAsync(newRelation);

        // Assert
        Assert.NotNull(created);
        Assert.Equal(newRelation.ClassId, created.ClassId);
        Assert.Equal(newRelation.UserId, created.UserId);
    }

    [Fact]
    public async Task Add_DuplicateRelation_ThrowsException()
    {
        // Arrange
        var user = await SeedUser();
        var cls = await SeedClass(user.Id);
        var newRelation = new UserClassRelationId { ClassId = cls.Id, UserId = user.Id };
        await _creator.AddAsync(newRelation);
        
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelation));
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        // Arrange
        var user = await SeedUser();
        var cls = await SeedClass(user.Id);
        var created = await SeedClassStudent(cls.Id, user.Id);

        // Act
        var found = await _reader.GetAsync(
            new() { ClassId = created.ClassId, UserId = created.UserId }
        );

        // Assert
        Assert.NotNull(found);
        Assert.Equal(created.UserId, found.UserId);
        Assert.Equal(created.ClassId, found.ClassId);
    }

    [Fact]
    public async Task Get_NonExistingRelation_ReturnsEmptyOptional()
    {
        // Act
        var found = await _reader.GetAsync(new() { ClassId = "non-existent", UserId = 99 });
        
        // Assert
        Assert.Null(found);
    }

    [Fact]
    public async Task Update_ExistingRelation_ReturnsUpdatedRelation()
    {
        // Arrange
        var user = await SeedUser();
        var cls = await SeedClass(user.Id);
        var created = await SeedClassStudent(cls.Id, user.Id);

        var toUpdate = new ClassStudentUpdateDTO
        {
            ClassId = created.ClassId,
            UserId = created.UserId,
            Hidden = true,
        };

        // Act
        var updated = await _updater.UpdateAsync(toUpdate);

        // Assert
        Assert.NotNull(updated);
        Assert.True(updated.Hidden);
    }

    [Fact]
    public async Task Update_NonExistingRelation_ThrowsException()
    {
        // Arrange
        var toUpdate = new ClassStudentUpdateDTO
        {
            ClassId = "non-existent",
            UserId = 99,
            Hidden = true,
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _updater.UpdateAsync(toUpdate));
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        // Arrange
        var user = await SeedUser();
        var cls = await SeedClass(user.Id);
        var created = await SeedClassStudent(cls.Id, user.Id);

        // Act
        var deleted = await _deleter.DeleteAsync(
            new() { ClassId = created.ClassId, UserId = created.UserId }
        );

        // Assert
        Assert.NotNull(deleted);
        Assert.Equal(created.ClassId, deleted.ClassId);
        Assert.Equal(created.UserId, deleted.UserId);

        var found = await _reader.GetAsync(
            new() { ClassId = created.ClassId, UserId = created.UserId }
        );
        Assert.Null(found);
    }

    [Fact]
    public async Task Delete_NonExistingRelation_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() =>
            _deleter.DeleteAsync(new() { ClassId = "non-existent", UserId = 99 })
        );
    }
}

