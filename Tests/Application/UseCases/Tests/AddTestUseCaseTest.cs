using Application.DTOs.Common;
using Application.DTOs.Tests;
using Application.UseCases.Tests;
using Domain.Entities.Questions;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Application.UseCases.Tests;

public class AddTestUseCaseTest : BaseTest
{
    private readonly AddTestUseCase _useCase;

    public AddTestUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<AddTestUseCase>();
    }

    private static Dictionary<Guid, IQuestion> GetValidTestContent()
    {
        return new Dictionary<Guid, IQuestion>
        {
            { Guid.NewGuid(), new OpenQuestion { Title = "Valid Question", ImageUrl = null } }
        };
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndAdminRole_ReturnsOk()
    {
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser(UserType.PROFESSOR);
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Color = "#ffffff",
            Content = GetValidTestContent(),
            ProfessorId = professor.Id,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = newTest, Executor = AsExecutor(admin) }
        );

        // Assert
        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndProfessorRole_ReturnsOk()
    {
        // Arrange
        var user = await SeedUser(UserType.PROFESSOR);
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Color = "#ffffff",
            Content = GetValidTestContent(),
            ProfessorId = user.Id,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = newTest, Executor = AsExecutor(user) }
        );

        // Assert
        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidProfessorId_ReturnsError()
    {
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Color = "#ffffff",
            Content = GetValidTestContent(),
            ProfessorId = 999,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = newTest, Executor = AsExecutor(admin) }
        );

        // Assert
        Assert.True(result.IsErr);
        var err = result.UnwrapErr();
        Assert.IsType<InputError>(err);
        Assert.Contains(((InputError)err).Errors, e => e.Field == "professorId");
    }

    [Fact]
    public async Task ExecuteAsync_WithStudentRole_ReturnsUnauthorizedError()
    {
        // Arrange
        var student = await SeedUser(UserType.STUDENT);
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Color = "#ffffff",
            Content = GetValidTestContent(),
            ProfessorId = student.Id,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = newTest, Executor = AsExecutor(student) }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_ProfessorCreatingForAnotherProfessor_ReturnsUnauthorizedError()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var unauthorizedProfessor = await SeedUser(UserType.PROFESSOR);
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Color = "#ffffff",
            Content = GetValidTestContent(),
            ProfessorId = professor.Id,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = newTest, Executor = AsExecutor(unauthorizedProfessor) }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }
}

