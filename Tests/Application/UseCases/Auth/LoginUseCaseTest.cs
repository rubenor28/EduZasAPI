using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.UseCases.Auth;
using Application.UseCases.Users;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.Auth;

public class LoginUseCaseTest : BaseTest
{
    private readonly LoginUseCase _useCase;
    private readonly AddUserUseCase _addUseCase;

    public LoginUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<LoginUseCase>();
        _addUseCase = _sp.GetRequiredService<AddUserUseCase>();
    }

    private async Task<UserDomain> SeedRegistration(string email, string pwd)
    {
        var admin = await SeedUser(UserType.ADMIN);
        return (
            await _addUseCase.ExecuteAsync(
                new()
                {
                    Data = new()
                    {
                        Email = email,
                        Password = pwd,
                        FirstName = "Test",
                        FatherLastname = "Test",
                        Role = UserType.STUDENT,
                    },
                    Executor = AsExecutor(admin),
                }
            )
        ).Unwrap();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ReturnsOk()
    {
        var email = "test@test.com";
        var pwd = "Contraseña1";
        await SeedRegistration(email, pwd);

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
        await SeedRegistration( email, pwd);

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
}
