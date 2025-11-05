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

    private readonly UserEFMapper _userMapper = new();
    private readonly TestEFMapper _testMapper = new();
    private readonly ClassEFMapper _classMapper = new();
    private readonly ClassTestEFMapper _classTestMapper = new();

    private readonly Random _random = new();

    public DeleteClassTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var classTestDeleter = new ClassTestEFDeleter(_ctx, _classTestMapper);
        var classTestReader = new ClassTestEFReader(_ctx, _classTestMapper);
        var testReader = new TestEFReader(_ctx, _testMapper);

        _useCase = new DeleteClassTestUseCase(classTestDeleter, classTestReader, testReader);
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.PROFESSOR)
    {
        var id = (ulong)_random.Next(1000, 100000);
        var user = new User
        {
            UserId = id,
            Email = $"user{id}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)role,
        };

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();

        return _userMapper.Map(user);
    }

    private async Task<TestDomain> SeedTest(ulong professorId)
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
        return _testMapper.Map(test);
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

    private static Executor AsExecutor(UserDomain user) => new() { Id = user.Id, Role = user.Role };

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndAdminRole_ReturnsOk()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser();
        var test = await SeedTest(professor.Id);
        var @class = await SeedClass(professor.Id);
        var classTest = await SeedClassTest(@class.Id, test.Id);

        var deleteDto = new DeleteClassTestDTO { Id = classTest.Id, Executor = AsExecutor(admin) };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsOk);
        Assert.NotNull(result.Unwrap());
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndProfessorRole_ReturnsOk()
    {
        var professor = await SeedUser();
        var test = await SeedTest(professor.Id);
        var @class = await SeedClass(professor.Id);
        var classTest = await SeedClassTest(@class.Id, test.Id);

        var deleteDto = new DeleteClassTestDTO
        {
            Id = classTest.Id,
            Executor = AsExecutor(professor),
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsOk);
        Assert.NotNull(result.Unwrap());
    }

    [Fact]
    public async Task ExecuteAsync_RelationNotFound_ReturnsError()
    {
        var admin = await SeedUser(UserType.ADMIN);

        var deleteDto = new DeleteClassTestDTO
        {
            Id = new ClassTestIdDTO { ClassId = "non-existent", TestId = 999 },
            Executor = AsExecutor(admin),
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithStudentRole_ReturnsUnauthorizedError()
    {
        var student = await SeedUser(UserType.STUDENT);
        var professor = await SeedUser();
        var test = await SeedTest(professor.Id);
        var @class = await SeedClass(professor.Id);
        var classTest = await SeedClassTest(@class.Id, test.Id);

        var deleteDto = new DeleteClassTestDTO
        {
            Id = classTest.Id,
            Executor = AsExecutor(student),
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_ProfessorDeletingAnotherProfessorsRelation_ReturnsUnauthorizedError()
    {
        var professor1 = await SeedUser();
        var professor2 = await SeedUser();
        var test = await SeedTest(professor1.Id);
        var @class = await SeedClass(professor1.Id);
        var classTest = await SeedClassTest(@class.Id, test.Id);

        var deleteDto = new DeleteClassTestDTO
        {
            Id = classTest.Id,
            Executor = AsExecutor(professor2),
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
