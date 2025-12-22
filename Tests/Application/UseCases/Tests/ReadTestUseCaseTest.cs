using Application.DTOs.Common;
using Application.UseCases.Tests;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Tests.Application.UseCases.Tests;

public class ReadTestUseCaseTest : BaseTest
{
    private readonly ReadTestUseCase _useCase;

    public ReadTestUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<ReadTestUseCase>();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidId_ReturnsOk()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var test = await SeedTest(professor.Id);

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = test.Id,
                Executor = new() { Id = professor.Id, Role = professor.Role },
            }
        );

        // Assert
        Assert.True(result.IsOk);
        var foundTest = result.Unwrap();
        Assert.NotNull(foundTest);
        Assert.Equal(test.Id, foundTest.Id);
    }

    [Fact]
    public async Task ExecuteAsync_TestNotFound_ReturnsError()
    {
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);

        // Act
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = Guid.NewGuid(),
                Executor = AsExecutor(admin),
            }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }
}

