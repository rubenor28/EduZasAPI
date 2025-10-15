
using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Tests.EntityFramework;

public class UserEFRepositoryTest : IDisposable
{
    private readonly IRepositoryAsync<ulong, UserDomain, NewUserDTO, UserUpdateDTO, DeleteUserDTO, UserCriteriaDTO> _repository;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public UserEFRepositoryTest()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>()
          .UseSqlite(_conn)
          .Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();
        _repository = new UserEntityFrameworkRepository(_ctx, 10);
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

        var created = await _repository.AddAsync(newUser);
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
        await _repository.AddAsync(newUser);

        var duplicateUser = new NewUserDTO
        {
            Email = "test@test.com", // Same email
            Password = "othersecurepwd1234",
            FirstName = "test2",
            FatherLastName = "test2",
        };

        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(duplicateUser));
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
        var created = await _repository.AddAsync(newUser);

        var update = new UserUpdateDTO
        {
            Id = created.Id,
            Email = "update@test.com",
            FirstName = "update",
            Password = "update",
            FatherLastName = "update",
            Active = false,
            MidName = "update".ToOptional(),
            MotherLastname = "update".ToOptional()
        };

        var updatedUser = await _repository.UpdateAsync(update);
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

        await _repository.AddAsync(otherUser);
        var created = await _repository.AddAsync(newUser);

        var updateEmailRepeated = new UserUpdateDTO
        {
            Id = created.Id,
            Email = "test2@test.com",
            FirstName = "update",
            Password = "update",
            FatherLastName = "update",
            Active = false,
            MidName = "update".ToOptional(),
            MotherLastname = "update".ToOptional()
        };

        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.UpdateAsync(updateEmailRepeated));
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
        var created = await _repository.AddAsync(newUser);

        var foundUser = await _repository.GetAsync(created.Id);

        Assert.True(foundUser.IsSome);
        Assert.Equal(created.Id, foundUser.Unwrap().Id);
    }

    [Fact]
    public async Task GetAsync_WhenUserDoesNotExist_ReturnsEmptyOptional()
    {
        var foundUser = await _repository.GetAsync(123);

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
        var created = await _repository.AddAsync(newUser);

        var deletedUser = await _repository.DeleteAsync(created.Id);

        Assert.NotNull(deletedUser);
        Assert.Equal(created.Id, deletedUser.Id);

        var foundUser = await _repository.GetAsync(created.Id);
        Assert.True(foundUser.IsNone);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserDoesNotExist_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.DeleteAsync(123));
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
        await _repository.AddAsync(newUser1);

        var newUser2 = new NewUserDTO
        {
            Email = "test2@test.com",
            Password = "securepwd1234",
            FirstName = "test2",
            FatherLastName = "test2",
        };
        await _repository.AddAsync(newUser2);

        var criteria = new UserCriteriaDTO
        {
            Email = Optional.Some(new StringQueryDTO { Text = "test1@test.com", SearchType = StringSearchType.EQ })
        };

        var result = await _repository.GetByAsync(criteria);

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

