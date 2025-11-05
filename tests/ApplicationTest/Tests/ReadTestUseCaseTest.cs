
using Application.DAOs;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Tests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Tests;

public class ReadTestUseCaseTest : IDisposable
{
    private readonly ReadTestUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public ReadTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testMapper = new TestEFMapper();
        var testReader = new TestEFReader(_ctx, testMapper);
        var validator = new InlineValidator<ulong>();

        _useCase = new ReadTestUseCase(testReader, validator);
    }

    private async Task<Test> SeedTest(ulong professorId)
    {
        var test = new Test
        {
            TestId = 1,
            Title = "Original Title",
            Description = "Original Description",
            ProfesorId = professorId
        };
        _ctx.Tests.Add(test);
        await _ctx.SaveChangesAsync();
        return test;
    }

    [Fact]
    public async Task ExecuteAsync_WithValidId_ReturnsOk()
    {
        var seeded = await SeedTest(1);

        var result = await _useCase.ExecuteAsync(seeded.TestId);

        Assert.True(result.IsOk);
        var foundTest = result.Unwrap();
        Assert.Equal(seeded.TestId, foundTest.Id);
    }

    [Fact]
    public async Task ExecuteAsync_TestNotFound_ReturnsError()
    {
        var result = await _useCase.ExecuteAsync(999);

        Assert.True(result.IsErr);
        Assert.Equal(typeof(NotFoundError), result.UnwrapErr().GetType());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
