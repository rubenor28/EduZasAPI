using Application.DTOs.Common;
using Application.DTOs.Tests;
using Application.UseCases.Tests;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Tests;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using FluentValidationProj.Application.Services.Tests;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Tests;

public class AddTestUseCaseTest : IDisposable
{
    private readonly AddTestUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly UserMapper _userMapper = new();

    private readonly Random _random = new();

    public AddTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testMapper = new TestMapper();

        var testCreator = new TestEFCreator(_ctx, testMapper, new NewTestEFMapper());
        var userReader = new UserEFReader(_ctx, _userMapper);
        var testValidator = new NewTestFluentValidator();

        _useCase = new AddTestUseCase(testCreator, userReader, testValidator);
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.PROFESSOR)
    {
        var id = _random.Next(1000, 100000);
        var user = new User
        {
            Email = $"test{id}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)role,
        };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return _userMapper.Map(user);
    }

    private static Executor AsExecutor(UserDomain user) => new() { Id = user.Id, Role = user.Role };

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndAdminRole_ReturnsOk()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser();
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test content",
            ProfessorId = professor.Id,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = newTest, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndProfessorRole_ReturnsOk()
    {
        var user = await SeedUser();
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = user.Id,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = newTest, Executor = AsExecutor(user) }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidProfessorId_ReturnsError()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = 999, // Non-existent professor
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = newTest, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsErr);
        var err = result.UnwrapErr();
        Assert.IsType<InputError>(err);
        Assert.Contains(((InputError)err).Errors, e => e.Field == "profesorId");
    }

    [Fact]
    public async Task ExecuteAsync_WithStudentRole_ReturnsUnauthorizedError()
    {
        var student = await SeedUser(UserType.STUDENT);
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = student.Id,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = newTest, Executor = AsExecutor(student) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_ProfessorCreatingForAnotherProfessor_ReturnsUnauthorizedError()
    {
        var professor = await SeedUser();
        var unauthorizedProfessor = await SeedUser();
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = professor.Id,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = newTest, Executor = AsExecutor(unauthorizedProfessor) }
        );

        Assert.True(result.IsErr);
        Assert.Equal(typeof(UnauthorizedError), result.UnwrapErr().GetType());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
