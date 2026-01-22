using Application.DTOs.Users;
using Application.UseCases.Users;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Tests;

public class AddUserUseCaseTest : BaseTest
{
    private readonly AddUserUseCase _useCase;

    public AddUserUseCaseTest()
    {
        var sp = ServiceProviderFactory.CreateServiceProvider();
        _useCase = sp.GetRequiredService<AddUserUseCase>();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ReturnsOk()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastname = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            Role = UserType.STUDENT,
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newUser,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateEmail_ReturnsError()
    {
        var existingUser = new NewUserDTO
        {
            FirstName = "JANE",
            FatherLastname = "DOE",
            Email = "jane.doe@example.com",
            Password = "Password123!",
            Role = UserType.STUDENT,
        };
        await _useCase.ExecuteAsync(
            new()
            {
                Data = existingUser,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

        var newUser = new NewUserDTO
        {
            FirstName = "JOHN",
            FatherLastname = "DOE",
            Email = "jane.doe@example.com", // Duplicate email
            Password = "Password123!",
            Role = UserType.STUDENT,
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newUser,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsErr);
        Assert.IsType<Conflict>(result.UnwrapErr());
        var err = (Conflict)result.UnwrapErr();
        Assert.Equal("El recurso ya existe", err.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidData_ReturnsError()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "", // Invalid first name
            FatherLastname = "DOE",
            Email = "john.doe@example.com",
            Password = "Password123!",
            Role = UserType.STUDENT,
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newUser,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsErr);
        var error = result.UnwrapErr();
        Assert.Equal(typeof(InputError), error.GetType());
        Assert.Contains(((InputError)error).Errors, e => e.Field == "firstName");
    }
}
