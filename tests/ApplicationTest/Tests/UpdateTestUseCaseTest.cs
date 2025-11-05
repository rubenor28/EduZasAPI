
using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Tests;
using Application.Services;
using Application.UseCases.Tests;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using FluentValidationProj.Application.Services.Tests;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Tests;

public class UpdateTestUseCaseTest : IDisposable
{
    private readonly UpdateTestUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public UpdateTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testMapper = new TestEFMapper();
        var testUpdater = new TestEFUpdater(_ctx, testMapper, testMapper);
        var testReader = new TestEFReader(_ctx, testMapper);
        var testValidator = new TestUpdateFluentValidator();

        _useCase = new UpdateTestUseCase(testUpdater, testReader, testValidator);
    }

    private async Task SeedUser(UserType role, ulong userId = 1)
    {
        var user = new User
        {
            UserId = userId,
            Email = "test@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)role,
        };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
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
    public async Task ExecuteAsync_WithValidDataAndAdminRole_ReturnsOk()
    {
        await SeedUser(UserType.ADMIN, 1);
        await SeedUser(UserType.PROFESSOR, 2);
        var seeded = await SeedTest(2);

        var updateDto = new TestUpdateDTO
        {
            Id = seeded.TestId,
            Title = "Updated Title",
            Description = "Updated Description",
            ProfesorId = 2,
            Executor = new Executor { Id = 1, Role = UserType.ADMIN }
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndProfessorRole_ReturnsOk()
    {
        await SeedUser(UserType.PROFESSOR, 1);
        var seeded = await SeedTest(1);

        var updateDto = new TestUpdateDTO
        {
            Id = seeded.TestId,
            Title = "Updated Title",
            Description = "Updated Description",
            ProfesorId = 1,
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_TestNotFound_ReturnsError()
    {
        await SeedUser(UserType.ADMIN, 1);

        var updateDto = new TestUpdateDTO
        {
            Id = 999, // Non-existent test
            Title = "Updated Title",
            Description = "Updated Description",
            ProfesorId = 1,
            Executor = new Executor { Id = 1, Role = UserType.ADMIN }
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        Assert.Equal(typeof(NotFoundError), result.UnwrapErr().GetType());
    }

    [Fact]
    public async Task ExecuteAsync_WithStudentRole_ReturnsUnauthorizedError()
    {
        await SeedUser(UserType.STUDENT, 1);
        var seeded = await SeedTest(1);

        var updateDto = new TestUpdateDTO
        {
            Id = seeded.TestId,
            Title = "Updated Title",
            Description = "Updated Description",
            ProfesorId = 1,
            Executor = new Executor { Id = 1, Role = UserType.STUDENT }
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        Assert.Equal(typeof(UnauthorizedError), result.UnwrapErr().GetType());
    }

    [Fact]
    public async Task ExecuteAsync_ProfessorUpdatingAnotherProfessorsTest_ReturnsUnauthorizedError()
    {
        await SeedUser(UserType.PROFESSOR, 1);
        await SeedUser(UserType.PROFESSOR, 2);
        var seeded = await SeedTest(2);

        var updateDto = new TestUpdateDTO
        {
            Id = seeded.TestId,
            Title = "Updated Title",
            Description = "Updated Description",
            ProfesorId = 2,
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        Assert.Equal(typeof(UnauthorizedError), result.UnwrapErr().GetType());
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidTitle_ReturnsInputError()
    {
        await SeedUser(UserType.PROFESSOR, 1);
        var seeded = await SeedTest(1);

        var updateDto = new TestUpdateDTO
        {
            Id = seeded.TestId,
            Title = "", // Invalid title
            Description = "Updated Description",
            ProfesorId = 1,
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        var err = result.UnwrapErr();
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "Title");
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
