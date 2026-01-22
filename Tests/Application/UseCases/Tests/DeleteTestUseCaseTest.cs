using Domain.ValueObjects;
using Application.UseCases.Tests;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.UseCases.Tests;

public class DeleteTestUseCaseTest : BaseTest
{
    private readonly DeleteTestUseCase _useCase;

    public DeleteTestUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<DeleteTestUseCase>();
    }
    
    [Fact]
    public async Task ExecuteAsync_WithValidId_ReturnsOk()
    {
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        var seeded = await SeedTest(admin.Id);

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = seeded.Id, Executor = AsExecutor(admin) }
        );

        // Assert
        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_TestNotFound_ReturnsError()
    {
        // Arrange
        var user = await SeedUser(UserType.ADMIN);

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = Guid.NewGuid(), Executor = AsExecutor(user) }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }
}

