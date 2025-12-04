using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Application.UseCases.ClassTests;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DAOs.ClassTests;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;
using EntityFramework.InterfaceAdapters.Mappers.ClassTests;
using EntityFramework.InterfaceAdapters.Mappers.Tests;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.ClassTests;

public class AddClassTestUseCaseTest : IDisposable
{
    private readonly AddClassTestUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;
    private readonly UserMapper _userMapper = new();
    private readonly ClassMapper _classMapper = new();
    private readonly TestMapper _testMapper = new();
    private readonly Random _rdm = new();

    public AddClassTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;
        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var classTestMapper = new ClassTestMapper();
        var professorClassMapper = new ClassProfessorMapper();

        var creator = new ClassTestEFCreator(_ctx, classTestMapper, new NewClassTestEFMapper());
        var testReader = new TestEFReader(_ctx, _testMapper);
        var classReader = new ClassEFReader(_ctx, _classMapper);
        var classTestReader = new ClassTestEFReader(_ctx, classTestMapper);
        var professorReader = new ClassProfessorsEFReader(_ctx, professorClassMapper);

        _useCase = new AddClassTestUseCase(
            creator,
            testReader,
            classReader,
            classTestReader,
            professorReader,
            null
        );
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.PROFESSOR)
    {
        var id = (ulong)_rdm.NextInt64(1, 1_000_000);
        var user = new User
        {
            UserId = id,
            Email = $"user-{id}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)role,
        };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return _userMapper.Map(user);
    }

    private async Task<ClassDomain> SeedClass()
    {
        var id = (ulong)_rdm.NextInt64(1, 1_000_000);
        var cls = new Class { ClassId = $"class-{id}", ClassName = "Test Class" };
        _ctx.Classes.Add(cls);
        await _ctx.SaveChangesAsync();
        return _classMapper.Map(cls);
    }

    private async Task<TestDomain> SeedTest(ulong professorId)
    {
        var test = new Test
        {
            TestId = Guid.NewGuid(),
            Title = "Test Title",
            ProfessorId = professorId,
            Content = "[]",
        };
        _ctx.Tests.Add(test);
        await _ctx.SaveChangesAsync();
        return _testMapper.Map(test);
    }

    private async Task SeedClassProfessor(string classId, ulong professorId)
    {
        var relation = new ClassProfessor
        {
            ClassId = classId,
            ProfessorId = professorId,
            IsOwner = true,
        };
        _ctx.ClassProfessors.Add(relation);
        await _ctx.SaveChangesAsync();
    }

    private static Executor AsExecutor(UserDomain value) =>
        new() { Id = value.Id, Role = value.Role };

    [Fact]
    public async Task ExecuteAsync_AsAdmin_WhenTestOwnerIsInClass_AddsSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser();
        var cls = await SeedClass();
        var test = await SeedTest(professor.Id);
        await SeedClassProfessor(cls.Id, professor.Id);

        var dto = new ClassTestDTO
        {
            ClassId = cls.Id,
            TestId = test.Id,
            Visible = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_AsAdmin_WhenTestOwnerIsNotInClass_ReturnsUnauthorized()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser();
        var cls = await SeedClass();
        var test = await SeedTest(professor.Id);
        // Note: Professor is NOT seeded into the class

        var dto = new ClassTestDTO
        {
            ClassId = cls.Id,
            TestId = test.Id,
            Visible = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AsProfessorAddingOwnTestAndInClass_AddsSuccessfully()
    {
        var professor = await SeedUser();
        var cls = await SeedClass();
        var test = await SeedTest(professor.Id);
        await SeedClassProfessor(cls.Id, professor.Id);

        var dto = new ClassTestDTO
        {
            ClassId = cls.Id,
            TestId = test.Id,
            Visible = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(professor) }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClass_ReturnsInputError()
    {
        var professor = await SeedUser();
        var test = await SeedTest(professor.Id);
        var dto = new ClassTestDTO
        {
            ClassId = "non-existent",
            TestId = test.Id,
            Visible = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(professor) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<InputError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenRelationAlreadyExists_ReturnsAlreadyExistsError()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser();
        var cls = await SeedClass();
        var test = await SeedTest(professor.Id);
        await SeedClassProfessor(cls.Id, professor.Id);

        var dto = new ClassTestDTO
        {
            ClassId = cls.Id,
            TestId = test.Id,
            Visible = true,
        };
        await _useCase.ExecuteAsync(new() { Data = dto, Executor = AsExecutor(admin) }); // First time

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        ); // Second time

        Assert.True(result.IsErr);
        Assert.IsType<AlreadyExistsError>(result.UnwrapErr());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
