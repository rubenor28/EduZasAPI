using Domain.ValueObjects;
using Application.DTOs.Tests;
using Application.UseCases.Tests;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.UseCases.Tests;

public class QueryTestUseCaseTest : BaseTest
{
    private readonly QueryTestUseCase _useCase;

    public QueryTestUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<QueryTestUseCase>();
    }

    private async Task<(Domain.Entities.UserDomain, Domain.Entities.UserDomain)> SeedTests()
    {
        var professor1 = await SeedUser(UserType.PROFESSOR);
        var professor2 = await SeedUser(UserType.PROFESSOR);

        await SeedTest(professor1.Id);
        await SeedTest(professor1.Id);
        await SeedTest(professor2.Id);
        
        return (professor1, professor2);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoCriteria_ReturnsAllTests()
    {
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        await SeedTests();
        var criteria = new TestCriteriaDTO();

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        // Assert
        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        Assert.Equal(3, search.Results.Count());
    }

    [Fact]
    public async Task ExecuteAsync_WithTitleCriteria_ReturnsMatchingTests()
    {
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        await SeedTests();
        var criteria = new TestCriteriaDTO
        {
            Title = new StringQueryDTO { Text = "Sample Test", SearchType = StringSearchType.EQ },
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        // Assert
        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        var results = search.Results;
        Assert.Equal(3, results.Count());
    }

    [Fact]
    public async Task ExecuteAsync_WithProfessorIdCriteria_ReturnsMatchingTests()
    {
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        var (_, professor2) = await SeedTests();
        var criteria = new TestCriteriaDTO { ProfessorId = professor2.Id };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        // Assert
        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        var results = search.Results;
        Assert.Single(results);
        Assert.Equal(professor2.Id, results.First().ProfessorId);
    }
}

