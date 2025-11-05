using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Application.UseCases.ClassTests;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.ClassTests;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.ClassTests;

public class DeleteClassTestUseCaseTest : IDisposable
{
    private readonly DeleteClassTestUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly ClassEFMapper _classMapper = new();
    private readonly ClassTestEFMapper _classTestMapper = new();

    public DeleteClassTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testMapper = new TestEFMapper();

        var classTestDeleter = new ClassTestEFDeleter(_ctx, _classTestMapper);
        var classTestReader = new ClassTestEFReader(_ctx, _classTestMapper);
        var testReader = new TestEFReader(_ctx, testMapper);

        _useCase = new DeleteClassTestUseCase(classTestDeleter, classTestReader, testReader);
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
            Content = "Original Content",
            ProfessorId = professorId,
        };
        _ctx.Tests.Add(test);
        await _ctx.SaveChangesAsync();
        return test;
    }

    private async Task<ClassDomain> SeedClass(ulong ownerId)
    {
        var classId = $"test-class-{ownerId}";
        var @class = new Class
        {
            ClassId = classId,
            Active = true,
            ClassName = "Test Class",
            Section = "A",
            Subject = "Math",
            Color = "#ffffff",
        };

        var classProfessor = new ClassProfessor
        {
            ClassId = classId,
            ProfessorId = ownerId,
            IsOwner = true,
        };

        _ctx.Classes.Add(@class);
        _ctx.ClassProfessors.Add(classProfessor);

        await _ctx.SaveChangesAsync();
        return _classMapper.Map(@class);
    }

    private async Task<ClassTestDomain> SeedClassTest(string classId, ulong testId)
    {
        var classTest = new TestPerClass
        {
            ClassId = classId,
            TestId = testId,
            Visible = true,
        };

        _ctx.TestsPerClasses.Add(classTest);
        await _ctx.SaveChangesAsync();
        return _classTestMapper.Map(classTest);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndAdminRole_ReturnsOk()
    {
        await SeedUser(UserType.ADMIN, 1);
        await SeedUser(UserType.PROFESSOR, 2);
        var test = await SeedTest(2);
        var @class = await SeedClass(2);
        await SeedClassTest(@class.Id, test.TestId);

        var deleteDto = new DeleteClassTestDTO
        {
            Id = new ClassTestIdDTO { ClassId = @class.Id, TestId = test.TestId },
            Executor = new Executor { Id = 1, Role = UserType.ADMIN },
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsOk);
        Assert.NotNull(result.Unwrap());
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndProfessorRole_ReturnsOk()
    {
        await SeedUser(UserType.PROFESSOR, 1);
        var test = await SeedTest(1);
        var @class = await SeedClass(1);
        await SeedClassTest(@class.Id, test.TestId);

        var deleteDto = new DeleteClassTestDTO
        {
            Id = new ClassTestIdDTO { ClassId = @class.Id, TestId = test.TestId },
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR },
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsOk);
        Assert.NotNull(result.Unwrap());
    }

    [Fact]
    public async Task ExecuteAsync_RelationNotFound_ReturnsError()
    {
        await SeedUser(UserType.ADMIN, 1);

        var deleteDto = new DeleteClassTestDTO
        {
            Id = new ClassTestIdDTO { ClassId = "non-existent", TestId = 999 },
            Executor = new Executor { Id = 1, Role = UserType.ADMIN },
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithStudentRole_ReturnsUnauthorizedError()
    {
        await SeedUser(UserType.STUDENT, 1);
        await SeedUser(UserType.PROFESSOR, 2);
        var test = await SeedTest(2);
        var @class = await SeedClass(2);
        await SeedClassTest(@class.Id, test.TestId);

        var deleteDto = new DeleteClassTestDTO
        {
            Id = new ClassTestIdDTO { ClassId = @class.Id, TestId = test.TestId },
            Executor = new Executor { Id = 1, Role = UserType.STUDENT },
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_ProfessorDeletingAnotherProfessorsRelation_ReturnsUnauthorizedError()
    {
        await SeedUser(UserType.PROFESSOR, 1);
        await SeedUser(UserType.PROFESSOR, 2);
        var test = await SeedTest(2);
        var @class = await SeedClass(2);
        await SeedClassTest(@class.Id, test.TestId);

        var deleteDto = new DeleteClassTestDTO
        {
            Id = new ClassTestIdDTO { ClassId = @class.Id, TestId = test.TestId },
            Executor = new Executor { Id = 1, Role = UserType.PROFESSOR },
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

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
