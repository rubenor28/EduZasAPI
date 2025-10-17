using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Enums;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest;

public class UserEFRepositoryTest : IDisposable
{
    private readonly SqliteConnection _conn;

    private readonly EduZasDotnetContext _ctx;
    private readonly UserEFCreator _creator;
    private readonly UserEFUpdater _updater;
    private readonly UserEFReader _reader;
    private readonly UserEFQuerier _querier;
    private readonly UserEFDeleter _deleter;

    public UserEFRepositoryTest()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var mapper = new UserEFMapper();

        _creator = new(_ctx, mapper, mapper);
        _updater = new(_ctx, mapper, mapper);
        _reader = new(_ctx, mapper);
        _deleter = new(_ctx, mapper);
        _querier = new(_ctx, mapper, 10);
    }

    [Fact]
    public async Task AddUser_RetunsUser()
    {
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastName = "test",
        };

        var created = await _creator.AddAsync(newUser);
        Assert.NotNull(created);
        Assert.Equal(created.Email, newUser.Email);
        Assert.Equal(created.Password, newUser.Password);
        Assert.Equal(created.FirstName, newUser.FirstName);
    }

    [Fact]
    public async Task AddUser_WithDuplicateEmail_ThrowsDbUpdateException()
    {
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastName = "test",
        };
        await _creator.AddAsync(newUser);

        var duplicateUser = new NewUserDTO
        {
            Email = "test@test.com", // Same email
            Password = "othersecurepwd1234",
            FirstName = "test2",
            FatherLastName = "test2",
        };

        await Assert.ThrowsAsync<DbUpdateException>(() => _creator.AddAsync(duplicateUser));
    }

    [Fact]
    public async Task UpdateUser_ReturnsUpdatedUser()
    {
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastName = "test",
        };
        var created = await _creator.AddAsync(newUser);

        var update = new UserUpdateDTO
        {
            Id = created.Id,
            Email = "update@test.com",
            FirstName = "update",
            Password = "update",
            FatherLastName = "update",
            Active = false,
            MidName = "update",
            MotherLastname = "update",
        };

        var updatedUser = await _updater.UpdateAsync(update);
        Assert.NotNull(updatedUser);
        Assert.Equal(created.Id, updatedUser.Id);
        Assert.Equal(update.Email, updatedUser.Email);
        Assert.Equal(update.FirstName, updatedUser.FirstName);
        Assert.Equal(update.Password, updatedUser.Password);
        Assert.Equal(update.FatherLastName, updatedUser.FatherLastName);
        Assert.Equal(update.Active, updatedUser.Active);
        Assert.Equal(update.MidName.Unwrap(), updatedUser.MidName.Unwrap());
        Assert.Equal(update.MotherLastname.Unwrap(), updatedUser.MotherLastname.Unwrap());
    }

    [Fact]
    public async Task UpdateUser_RepeatedEmail_ThrowsDbUpdateException()
    {
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastName = "test",
        };

        var otherUser = new NewUserDTO
        {
            Email = "test2@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastName = "test",
        };

        await _creator.AddAsync(otherUser);
        var created = await _creator.AddAsync(newUser);

        var updateEmailRepeated = new UserUpdateDTO
        {
            Id = created.Id,
            Email = "test2@test.com",
            FirstName = "update",
            Password = "update",
            FatherLastName = "update",
            Active = false,
            MidName = "update",
            MotherLastname = "update",
        };

        await Assert.ThrowsAsync<DbUpdateException>(() =>
            _updater.UpdateAsync(updateEmailRepeated)
        );
    }

    [Fact]
    public async Task GetAsync_WhenUserExists_ReturnsUser()
    {
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastName = "test",
        };
        var created = await _creator.AddAsync(newUser);

        var foundUser = await _reader.GetAsync(created.Id);

        Assert.True(foundUser.IsSome);
        Assert.Equal(created.Id, foundUser.Unwrap().Id);
    }

    [Fact]
    public async Task GetAsync_WhenUserDoesNotExist_ReturnsEmptyOptional()
    {
        var foundUser = await _reader.GetAsync(123);

        Assert.True(foundUser.IsNone);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserExists_ReturnsDeletedUser()
    {
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastName = "test",
        };
        var created = await _creator.AddAsync(newUser);

        var deletedUser = await _deleter.DeleteAsync(created.Id);

        Assert.NotNull(deletedUser);
        Assert.Equal(created.Id, deletedUser.Id);

        var foundUser = await _reader.GetAsync(created.Id);
        Assert.True(foundUser.IsNone);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserDoesNotExist_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _deleter.DeleteAsync(123));
    }

    [Fact]
    public async Task GetByAsync_WithEmailCriteria_ReturnsMatchingUser()
    {
        var newUser1 = new NewUserDTO
        {
            Email = "test1@test.com",
            Password = "securepwd1234",
            FirstName = "test1",
            FatherLastName = "test1",
        };
        await _creator.AddAsync(newUser1);

        var newUser2 = new NewUserDTO
        {
            Email = "test2@test.com",
            Password = "securepwd1234",
            FirstName = "test2",
            FatherLastName = "test2",
        };
        await _creator.AddAsync(newUser2);

        var criteria = new UserCriteriaDTO
        {
            Email = new StringQueryDTO
            {
                Text = "test1@test.com",
                SearchType = StringSearchType.EQ,
            },
        };

        var result = await _querier.GetByAsync(criteria);

        Assert.Single(result.Results);
        Assert.Equal(newUser1.Email, result.Results.First().Email);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
