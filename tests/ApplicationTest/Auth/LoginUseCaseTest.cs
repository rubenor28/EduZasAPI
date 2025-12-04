using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.UseCases.Auth;
using Application.UseCases.Users;
using Bcrypt.Application.Services;
using Domain.Enums;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using FluentValidationProj.Application.Services.Auth;
using InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Auth;

public class LoginUseCaseTest : IDisposable
{
    private readonly LoginUseCase _useCase;
    private readonly AddUserUseCase _addUserUseCase;
    private readonly SqliteConnection _conn;
    private readonly EduZasDotnetContext _ctx;

    public LoginUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var hasher = new BCryptHasher();
        var userMapper = new UserMapper();

        var userReader = new UserEmailEFReader(_ctx, userMapper);
        var credentialsValidator = new UserCredentialsFluentValidator();
        var creator = new UserEFCreator(
            _ctx,
            userMapper,
            new NewUserEFMapper(new UserTypeUintMapper())
        );
        var newUserValidator = new NewUserFluentValidator();

        _useCase = new LoginUseCase(hasher, userReader, credentialsValidator);
        _addUserUseCase = new AddUserUseCase(hasher, creator, newUserValidator, userReader);
    }

    private async Task SeedUser(string email, string password)
    {
        var newUser = new NewUserDTO
        {
            FirstName = "Test",
            FatherLastname = "Test",
            Email = email,
            Password = password,
            Role = UserType.STUDENT,
        };

        await _addUserUseCase.ExecuteAsync(
            new()
            {
                Data = newUser,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ReturnsOk()
    {
        var email = "john.doe@example.com";
        var pwd = "Password123!";
        await SeedUser(email, pwd);

        var credentials = new UserCredentialsDTO { Email = email, Password = pwd };

        var result = await _useCase.ExecuteAsync(credentials);

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ReturnsError()
    {
        var credentials = new UserCredentialsDTO
        {
            Email = "non.existent@example.com",
            Password = "Password123!",
        };

        var result = await _useCase.ExecuteAsync(credentials);

        var err = result.UnwrapErr();
        Assert.IsType<NotFoundError>(err);
    }

    [Fact]
    public async Task ExecuteAsync_WithIncorrectPassword_ReturnsError()
    {
        var email = "john.doe@example.com";
        var pwd = "Password123!";
        await SeedUser(email, pwd);

        var credentials = new UserCredentialsDTO { Email = email, Password = "IncorrectPassword!" };

        var result = await _useCase.ExecuteAsync(credentials);

        var err = result.UnwrapErr();
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "password");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidCredentials_ReturnsError()
    {
        var credentials = new UserCredentialsDTO { Email = "invalid-email", Password = "" };

        var result = await _useCase.ExecuteAsync(credentials);

        var err = result.UnwrapErr();
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "email");
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
