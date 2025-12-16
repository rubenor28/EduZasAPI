using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using InterfaceAdapters.Mappers.Users;
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
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var mapper = new UserMapper();
        var projector = new UserProjector();
        var roleMapper = new UserTypeUintMapper();

        _creator = new(_ctx, mapper, new NewUserEFMapper(roleMapper));
        _updater = new(_ctx, mapper, new UpdateUserEFMapper(roleMapper));
        _reader = new(_ctx, mapper);
        _deleter = new(_ctx, mapper);
        _querier = new(_ctx, projector, 10);
    }

    public static Executor AsExecutor(UserDomain user) => new() { Id = user.Id, Role = user.Role };

    [Fact]
    public async Task AddUser_RetunsUser()
    {
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastname = "test",
            Role = UserType.STUDENT,
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
            FatherLastname = "test",
            Role = UserType.STUDENT,
        };
        var created = await _creator.AddAsync(newUser);

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

        var updatedUser = await _updater.UpdateAsync(update);
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
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastname = "test",
            Role = UserType.STUDENT,
        };

        var created = await _creator.AddAsync(newUser);

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

        var updated = await _updater.UpdateAsync(updateEmailRepeated);
        Assert.Equal(created.Email, updated.Email);
    }

    [Fact]
    public async Task GetAsync_WhenUserExists_ReturnsUser()
    {
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastname = "test",
            Role = UserType.STUDENT,
        };
        var created = await _creator.AddAsync(newUser);

        var foundUser = await _reader.GetAsync(created.Id);

        Assert.NotNull(foundUser);
        Assert.Equal(created.Id, foundUser.Id);
    }

    [Fact]
    public async Task GetAsync_WhenUserDoesNotExist_ReturnsEmptyOptional()
    {
        var foundUser = await _reader.GetAsync(123);
        Assert.Null(foundUser);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserExists_ReturnsDeletedUser()
    {
        var newUser = new NewUserDTO
        {
            Email = "test@test.com",
            Password = "securepwd1234",
            FirstName = "test",
            FatherLastname = "test",
            Role = UserType.STUDENT,
        };
        var created = await _creator.AddAsync(newUser);

        var deletedUser = await _deleter.DeleteAsync(created.Id);

        Assert.NotNull(deletedUser);
        Assert.Equal(created.Id, deletedUser.Id);

        var foundUser = await _reader.GetAsync(created.Id);
        Assert.Null(foundUser);
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
            FatherLastname = "test1",
            Role = UserType.STUDENT,
        };
        await _creator.AddAsync(newUser1);

        var newUser2 = new NewUserDTO
        {
            Email = "test2@test.com",
            Password = "securepwd1234",
            FirstName = "test2",
            FatherLastname = "test2",
            Role = UserType.STUDENT,
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
