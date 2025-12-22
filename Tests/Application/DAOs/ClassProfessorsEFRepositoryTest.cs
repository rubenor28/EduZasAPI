using Application.DAOs;
using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

public class ClassProfessorsEFRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<ClassProfessorDomain, NewClassProfessorDTO> _creator;
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _reader;
    private readonly IUpdaterAsync<ClassProfessorDomain, ClassProfessorUpdateDTO> _updater;
    private readonly IDeleterAsync<UserClassRelationId, ClassProfessorDomain> _deleter;

    public ClassProfessorsEFRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<ClassProfessorDomain, NewClassProfessorDTO>>();
        _reader = _sp.GetRequiredService<IReaderAsync<UserClassRelationId, ClassProfessorDomain>>();
        _updater = _sp.GetRequiredService<IUpdaterAsync<ClassProfessorDomain, ClassProfessorUpdateDTO>>();
        _deleter = _sp.GetRequiredService<IDeleterAsync<UserClassRelationId, ClassProfessorDomain>>();
    }

    [Fact]
    public async Task Add_ValidRelation_ReturnsRelation()
    {
        // Arrange
        var user = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(user.Id);

        var newRelation = new NewClassProfessorDTO()
        {
            ClassId = cls.Id,
            UserId = user.Id,
            IsOwner = true,
        };

        // Act
        var created = await _creator.AddAsync(newRelation);

        // Assert
        Assert.NotNull(created);
        Assert.Equal(newRelation.ClassId, created.ClassId);
        Assert.Equal(newRelation.UserId, created.UserId);
        Assert.Equal(newRelation.IsOwner, created.IsOwner);
    }

    [Fact]
    public async Task Add_DuplicateRelation_ThrowsException()
    {
        // Arrange
        var user = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(user.Id);

        var newRelation = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            IsOwner = true,
        };

        await _creator.AddAsync(newRelation);

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelation));
    }

    [Fact]
    public async Task Add_NonExistentClass_ThrowsException()
    {
        // Arrange
        var user = await SeedUser(UserType.PROFESSOR);

        var newRelation = new NewClassProfessorDTO
        {
            ClassId = "non-existent-class",
            UserId = user.Id,
            IsOwner = true,
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelation));
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        // Arrange
        var user = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(user.Id);
        var created = await SeedClassProfessor(cls.Id, user.Id, true);

        // Act
        var found = await _reader.GetAsync(
            new() { ClassId = created.ClassId, UserId = created.UserId }
        );

        // Assert
        Assert.NotNull(found);
        Assert.Equal(created.ClassId, found.ClassId);
        Assert.Equal(created.UserId, found.UserId);
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
        var user = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(user.Id);
        var created = await SeedClassProfessor(cls.Id, user.Id, false);

        var updateDto = new ClassProfessorUpdateDTO
        {
            UserId = created.UserId,
            ClassId = created.ClassId,
            IsOwner = true,
        };

        // Act
        var updated = await _updater.UpdateAsync(updateDto);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(updateDto.ClassId, updated.ClassId);
        Assert.Equal(updateDto.UserId, updated.UserId);
        Assert.Equal(updateDto.IsOwner, updated.IsOwner);
    }

    [Fact]
    public async Task Update_NonExistingRelation_ThrowsException()
    {
        // Arrange
        var updateDto = new ClassProfessorUpdateDTO
        {
            ClassId = "non-existent",
            UserId = 99,
            IsOwner = true,
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _updater.UpdateAsync(updateDto));
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        // Arrange
        var user = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(user.Id);

        var created = await SeedClassProfessor(cls.Id, user.Id, false);
        var id = new UserClassRelationId { ClassId = created.ClassId, UserId = created.UserId };

        // Act
        var deleted = await _deleter.DeleteAsync(id);

        // Assert
        Assert.NotNull(deleted);
        Assert.Equal(created.ClassId, deleted.ClassId);
        Assert.Equal(created.UserId, deleted.UserId);

        var found = await _reader.GetAsync(id);
        Assert.Null(found);
    }

    [Fact]
    public async Task Delete_NonExistingRelation_ThrowsException()
    {
        // Arrange
        var id = new UserClassRelationId { ClassId = "non-existent", UserId = 99 };

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _deleter.DeleteAsync(id));
    }
}

