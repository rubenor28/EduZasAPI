using Application.DTOs.Common;
using Application.UseCases.Auth;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using FluentValidationProj.Application.Services.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Auth;

public class ReadUserUseCaseTest : IDisposable
{
    private readonly ReadUserUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;
    private readonly UserProjector _userMapper = new();
    private readonly Random _rdm = new();

    public ReadUserUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;
        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var userReader = new UserEFReader(_ctx, _userMapper);
        var validator = new ULongFluentValidator();

        _useCase = new ReadUserUseCase(userReader, validator);
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.STUDENT)
    {
        var id = (ulong)_rdm.NextInt64(1, 100_000);
        var user = new User
        {
            UserId = id,
            Email = $"user-{id}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)role,
        };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return _userMapper.Map(user);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingUser_ReturnsOkWithUserData()
    {
        // Arrange
        var seededUser = await SeedUser();

        // Act
        var result = await _useCase.ExecuteAsync(seededUser.Id);

        // Assert
        Assert.True(result.IsOk);
        var foundUser = result.Unwrap();
        Assert.Equal(seededUser.Id, foundUser.Id);
        Assert.Equal(seededUser.Email, foundUser.Email);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ReturnsNotFoundError()
    {
        // Arrange
        var nonExistentId = (ulong)_rdm.NextInt64(1, 100_000);

        // Act
        var result = await _useCase.ExecuteAsync(nonExistentId);

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidId_ReturnsInputError()
    {
        // Arrange
        const ulong invalidId = 0;

        // Act
        var result = await _useCase.ExecuteAsync(invalidId);

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<InputError>(result.UnwrapErr());
        var error = result.UnwrapErr() as InputError;
        Assert.NotNull(error);
        Assert.Contains(error.Errors, e => e.Field == "id" && e.Message == "Id debe ser mayor a 0");
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
