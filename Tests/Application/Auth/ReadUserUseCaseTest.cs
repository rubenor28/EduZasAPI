using Application.UseCases.Auth;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.Auth;

public class ReadUserUseCaseTest : BaseTest
{
    private readonly ReadUserUseCase _useCase;

    public ReadUserUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<ReadUserUseCase>();
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingUser_ReturnsOkWithUserData()
    {
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        var seededUser = await SeedUser();

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = seededUser.Id, Executor = AsExecutor(admin) }
        );

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
        var admin = await SeedUser(UserType.ADMIN);
        var nonExistentId = (ulong)_random.NextInt64(1, 100_000);

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = nonExistentId, Executor = AsExecutor(admin) }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidId_ReturnsInputError()
    {
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        const ulong invalidId = 0;

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = invalidId, Executor = AsExecutor(admin) }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<InputError>(result.UnwrapErr());
        var error = result.UnwrapErr() as InputError;
        Assert.NotNull(error);
        Assert.Contains(error.Errors, e => e.Field == "id" && e.Message == "Id debe ser mayor a 0");
    }
}

