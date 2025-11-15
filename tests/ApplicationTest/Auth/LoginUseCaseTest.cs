using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.UseCases.Auth;
using Bcrypt.Application.Services;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
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

        var roleMapper = new UserTypeMapper();
        var userMapper = new UserEFMapper(roleMapper, roleMapper);

        var querier = new UserEFQuerier(_ctx, userMapper, 10);
        var credentialsValidator = new UserCredentialsFluentValidator();
        var creator = new UserEFCreator(_ctx, userMapper, userMapper);
        var newUserValidator = new NewUserFluentValidator();

        _useCase = new LoginUseCase(hasher, querier, credentialsValidator);
        _addUserUseCase = new AddUserUseCase(hasher, creator, newUserValidator, querier);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ReturnsOk()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastname = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>(),
        };
        await _addUserUseCase.ExecuteAsync(newUser);

        var credentials = new UserCredentialsDTO
        {
            Email = "john.doe@example.com",
            Password = "Password123!",
        };

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
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "email");
    }

    [Fact]
    public async Task ExecuteAsync_WithIncorrectPassword_ReturnsError()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastname = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>(),
        };
        await _addUserUseCase.ExecuteAsync(newUser);

        var credentials = new UserCredentialsDTO
        {
            Email = "john.doe@example.com",
            Password = "IncorrectPassword!",
        };

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
