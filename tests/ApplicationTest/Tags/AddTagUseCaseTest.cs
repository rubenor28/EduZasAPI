using Application.DTOs.Common;
using Application.DTOs.Tags;
using Application.UseCases.Tags;
using Domain.Enums;
using EntityFramework.Application.DAOs.Tags;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Tags;

public class AddTagUseCaseTest : IDisposable
{
    private readonly AddTagUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;
    private readonly TagEFQuerier _querier;

    public AddTagUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var mapper = new TagEFMapper();
        var creator = new TagEFCreator(_ctx, mapper, mapper);
        _querier = new TagEFQuerier(_ctx, mapper, 10);

        _useCase = new AddTagUseCase(creator, _querier, null);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ReturnsOkAndUppercasesText()
    {
        var newTag = new NewTagDTO
        {
            Text = "new tag"
        };

        var result = await _useCase.ExecuteAsync(newTag);

        Assert.True(result.IsOk);

        // Verify it was saved and uppercased
        var queryResult = await _querier.GetByAsync(new() 
        {
            Text = new StringQueryDTO { Text = "NEW TAG", SearchType = StringSearchType.EQ }
        });
        
        Assert.Single(queryResult.Results);
        Assert.Equal("NEW TAG", queryResult.Results.First().Text);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateText_ReturnsError()
    {
        var firstTag = new NewTagDTO { Text = "duplicate tag" };
        await _useCase.ExecuteAsync(firstTag);

        var secondTag = new NewTagDTO { Text = "DUPLICATE TAG" }; // Different case
        var result = await _useCase.ExecuteAsync(secondTag);

        Assert.True(result.IsErr);
        var err = result.UnwrapErr();
        Assert.IsType<AlreadyExistsError>(err);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyTextAndNoValidator_ReturnsOk()
    {
        // This test documents the current behavior: without a validator, empty text is allowed.
        var newTag = new NewTagDTO { Text = "" };

        var result = await _useCase.ExecuteAsync(newTag);

        Assert.True(result.IsOk);

        // Verify it was saved with empty text
        var queryResult = await _querier.GetByAsync(new() 
        {
            Text = new StringQueryDTO { Text = "", SearchType = StringSearchType.EQ }
        });
        Assert.Single(queryResult.Results);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
