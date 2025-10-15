
using EduZasAPI.Application.Auth;
using EduZasAPI.Application.Users;
using EduZasAPI.Domain.Common;
using EduZasAPI.Infraestructure.Bcrypt.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;
using EduZasAPI.Infraestructure.FluentValidation.Application.Auth;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Application.Tests;

public class LoginUseCaseTest : IDisposable
{
    private readonly LoginUseCase _useCase;
    private readonly AddUserUseCase _addUserUseCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public LoginUseCaseTest()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>()
          .UseSqlite(_conn)
          .Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var hasher = new BCryptHasher();
        var repository = new UserEntityFrameworkRepository(_ctx, 10);
        var userValidator = new NewUserFluentValidator();
        var credentialsValidator = new UserCredentialsFluentValidator();

        _useCase = new LoginUseCase(hasher, repository, credentialsValidator);
        _addUserUseCase = new AddUserUseCase(hasher, repository, userValidator, repository);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ReturnsOk()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>()
        };
        await _addUserUseCase.ExecuteAsync(newUser);

        var credentials = new UserCredentialsDTO
        {
            Email = "john.doe@example.com",
            Password = "Password123!"
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
            Password = "Password123!"
        };

        var result = await _useCase.ExecuteAsync(credentials);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "email");
    }

    [Fact]
    public async Task ExecuteAsync_WithIncorrectPassword_ReturnsError()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastName = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            MidName = Optional.None<string>(),
            MotherLastname = Optional.None<string>()
        };
        await _addUserUseCase.ExecuteAsync(newUser);

        var credentials = new UserCredentialsDTO
        {
            Email = "john.doe@example.com",
            Password = "IncorrectPassword!"
        };

        var result = await _useCase.ExecuteAsync(credentials);

        Assert.True(result.IsErr);
        Assert.Contains(result.UnwrapErr(), e => e.Field == "password");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidCredentials_ReturnsError()
    {
        var credentials = new UserCredentialsDTO
        {
            Email = "invalid-email",
            Password = ""
        };

        var result = await _useCase.ExecuteAsync(credentials);

        Assert.True(result.IsErr);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
