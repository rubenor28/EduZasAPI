using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

public class UserEFRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<UserDomain, NewUserDTO> _creator;
    private readonly IUpdaterAsync<UserDomain, UserUpdateDTO> _updater;
    private readonly IReaderAsync<ulong, UserDomain> _reader;
    private readonly IQuerierAsync<UserDomain, UserCriteriaDTO> _querier;
    private readonly IDeleterAsync<ulong, UserDomain> _deleter;

    public UserEFRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<UserDomain, NewUserDTO>>();
        _updater = _sp.GetRequiredService<IUpdaterAsync<UserDomain, UserUpdateDTO>>();
        _reader = _sp.GetRequiredService<IReaderAsync<ulong, UserDomain>>();
        _querier = _sp.GetRequiredService<IQuerierAsync<UserDomain, UserCriteriaDTO>>();
        _deleter = _sp.GetRequiredService<IDeleterAsync<ulong, UserDomain>>();
    }

    [Fact]
    public async Task AddUser_RetunsUser()
    {
        // Arrange
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastname = "test",
            Role = UserType.STUDENT,
        };

        // Act
        var created = await _creator.AddAsync(newUser);
        
        // Assert
        Assert.NotNull(created);
        Assert.Equal(created.Email, newUser.Email);
        Assert.Equal(created.Password, newUser.Password);
        Assert.Equal(created.FirstName, newUser.FirstName);
    }

    [Fact]
    public async Task AddUser_WithDuplicateEmail_ThrowsDbUpdateException()
    {
        // Arrange
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastname = "test",
            Role = UserType.STUDENT,
        };
        await _creator.AddAsync(newUser);

        var duplicateUser = new NewUserDTO
        {
            Email = "test@test.com", // Same email
            Password = "othersecurepwd1234",
            FirstName = "test2",
            FatherLastname = "test2",
            Role = UserType.STUDENT,
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<Microsoft.EntityFrameworkCore.DbUpdateException>(() => _creator.AddAsync(duplicateUser));
    }

    [Fact]
    public async Task UpdateUser_ReturnsUpdatedUser()
    {
        // Arrange
        var created = await SeedUser(UserType.STUDENT, email: "test@test.com");

        var update = new UserUpdateDTO
        {
            Id = created.Id,
            Email = created.Email,
            FirstName = "update",
            Password = "update",
            FatherLastname = "update",
            Active = false,
            MidName = "update",
            MotherLastname = "update",
            Role = UserType.PROFESSOR,
        };

        // Act
        var updatedUser = await _updater.UpdateAsync(update);
        
        // Assert
        Assert.NotNull(updatedUser);
        Assert.Equal(created.Id, updatedUser.Id);
        Assert.Equal(update.Email, updatedUser.Email);
        Assert.Equal(update.FirstName, updatedUser.FirstName);
        Assert.Equal(update.Password, updatedUser.Password);
        Assert.Equal(update.FatherLastname, updatedUser.FatherLastname);
        Assert.Equal(update.Active, updatedUser.Active);
        Assert.Equal(update.MidName, updatedUser.MidName);
        Assert.Equal(update.MotherLastname, updatedUser.MotherLastname);
    }

    [Fact]
    public async Task UpdateUser_UpdateEmail_EmailShouldDontChange()
    {
        // Arrange
        var created = await SeedUser(UserType.STUDENT, email: "test@test.com");

        var updateEmailRepeated = new UserUpdateDTO
        {
            Id = created.Id,
            Email = "test2@test.com",
            FirstName = "update",
            Password = "update",
            FatherLastname = "update",
            Active = false,
            MidName = "update",
            MotherLastname = "update",
            Role = created.Role,
        };

        // Act
        var updated = await _updater.UpdateAsync(updateEmailRepeated);
        
        // Assert
        Assert.Equal(created.Email, updated.Email);
    }

    [Fact]
    public async Task GetAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var created = await SeedUser(UserType.STUDENT);

        // Act
        var foundUser = await _reader.GetAsync(created.Id);

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(created.Id, foundUser.Id);
    }

    [Fact]
    public async Task GetAsync_WhenUserDoesNotExist_ReturnsEmptyOptional()
    {
        // Act
        var foundUser = await _reader.GetAsync(123);
        
        // Assert
        Assert.Null(foundUser);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserExists_ReturnsDeletedUser()
    {
        // Arrange
        var created = await SeedUser(UserType.STUDENT);

        // Act
        var deletedUser = await _deleter.DeleteAsync(created.Id);
        var foundUser = await _reader.GetAsync(created.Id);

        // Assert
        Assert.NotNull(deletedUser);
        Assert.Equal(created.Id, deletedUser.Id);
        Assert.Null(foundUser);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserDoesNotExist_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _deleter.DeleteAsync(123));
    }

    [Fact]
    public async Task GetByAsync_WithEmailCriteria_ReturnsMatchingUser()
    {
        // Arrange
        await SeedUser(UserType.STUDENT, email: "test1@test.com");
        await SeedUser(UserType.STUDENT, email: "test2@test.com");

        var criteria = new UserCriteriaDTO
        {
            Email = new StringQueryDTO
            {
                Text = "test1@test.com",
                SearchType = StringSearchType.EQ,
            },
        };

        // Act
        var result = await _querier.GetByAsync(criteria);

        // Assert
        Assert.Single(result.Results);
        Assert.Equal("test1@test.com", result.Results.First().Email);
    }
}

