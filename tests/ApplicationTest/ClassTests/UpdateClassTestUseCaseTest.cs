
using Application.DAOs;
using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.ClassTests;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.ClassTests;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using FluentValidationProj.Application.Services.ClassTests;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.ClassTests;

public class UpdateClassTestUseCaseTest : IDisposable
{
    private readonly UpdateClassTestUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public UpdateClassTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var classTestMapper = new ClassTestEFMapper();
        var testMapper = new TestEFMapper();

        var classTestUpdater = new ClassTestEFUpdater(_ctx, classTestMapper, classTestMapper);
        var classTestReader = new ClassTestEFReader(_ctx, classTestMapper);
        var testReader = new TestEFReader(_ctx, testMapper);
        var validator = new ClassTestUpdateFluentValidator();

        _useCase = new UpdateClassTestUseCase(classTestUpdater, classTestReader, testReader, validator);
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

    private async Task<Class> SeedClass(ulong ownerId)
    {
        var @class = new Class
        {
            ClassId = 1,
            ClassName = "Test Class",
            Section = "A",
            Subject = "Math",
            PublicId = "test-class",
            OwnerId = ownerId
        };
        _ctx.Classes.Add(@class);
        await _ctx.SaveChangesAsync();
        return @class;
    }

    private async Task<ClassTest> SeedClassTest(ulong classId, ulong testId)
    {
        var classTest = new ClassTest
        {
            ClassId = classId,
            TestId = testId,
            OpenDate = DateTime.UtcNow,
            CloseDate = DateTime.UtcNow.AddDays(1)
        };
        _ctx.ClassTests.Add(classTest);
        await _ctx.SaveChangesAsync();
        return classTest;
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndAdminRole_ReturnsOk()
    {
        await SeedUser(UserType.ADMIN, 1);
        await SeedUser(UserType.PROFESSOR, 2);
        var test = await SeedTest(2);
        var @class = await SeedClass(2);
        var classTest = await SeedClassTest(@class.ClassId, test.TestId);

        var updateDto = new ClassTestUpdateDTO
        {
            Id = new ClassTestIdDTO { ClassId = @class.ClassId, TestId = test.TestId },
            OpenDate = DateTime.UtcNow.AddDays(2),
            CloseDate = DateTime.UtcNow.AddDays(3),
            Executor = new Executor { Id = 1, Role = UserType.ADMIN }
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndProfessorRole_ReturnsOk()
    {
        await SeedUser(UserType.PROFESSOR, 1);
        var test = await SeedTest(1);
        var @class = await SeedClass(1);
        var classTest = await SeedClassTest(@class.ClassId, test.TestId);

        var updateDto = new ClassTestUpdateDTO
        {
            Id = new ClassTestIdDTO { ClassId = @class.ClassId, TestId = test.TestId },
            OpenDate = DateTime.UtcNow.AddDays(2),
            CloseDate = DateTime.UtcNow.AddDays(3),
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_RelationNotFound_ReturnsError()
    {
        await SeedUser(UserType.ADMIN, 1);

        var updateDto = new ClassTestUpdateDTO
        {
            Id = new ClassTestIdDTO { ClassId = 999, TestId = 999 },
            OpenDate = DateTime.UtcNow.AddDays(2),
            CloseDate = DateTime.UtcNow.AddDays(3),
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
        await SeedUser(UserType.PROFESSOR, 2);
        var test = await SeedTest(2);
        var @class = await SeedClass(2);
        var classTest = await SeedClassTest(@class.ClassId, test.TestId);

        var updateDto = new ClassTestUpdateDTO
        {
            Id = new ClassTestIdDTO { ClassId = @class.ClassId, TestId = test.TestId },
            OpenDate = DateTime.UtcNow.AddDays(2),
            CloseDate = DateTime.UtcNow.AddDays(3),
            Executor = new Executor { Id = 1, Role = UserType.STUDENT }
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        Assert.Equal(typeof(UnauthorizedError), result.UnwrapErr().GetType());
    }

    [Fact]
    public async Task ExecuteAsync_ProfessorUpdatingAnotherProfessorsRelation_ReturnsUnauthorizedError()
    {
        await SeedUser(UserType.PROFESSOR, 1);
        await SeedUser(UserType.PROFESSOR, 2);
        var test = await SeedTest(2);
        var @class = await SeedClass(2);
        var classTest = await SeedClassTest(@class.ClassId, test.TestId);

        var updateDto = new ClassTestUpdateDTO
        {
            Id = new ClassTestIdDTO { ClassId = @class.ClassId, TestId = test.TestId },
            OpenDate = DateTime.UtcNow.AddDays(2),
            CloseDate = DateTime.UtcNow.AddDays(3),
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(updateDto);

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
