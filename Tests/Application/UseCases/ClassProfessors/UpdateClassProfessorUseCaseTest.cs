using Application.DTOs.ClassProfessors;
using Application.UseCases.ClassProfessors;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests.Application.UseCases.ClassProfessors;

public class UpdateClassProfessorUseCaseTest : IDisposable
{
    private readonly UpdateClassProfessorUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;
    private readonly UserMapper _userMapper = new();
    private readonly ClassMapper _classMapper = new();
    private readonly Random _rdm = new();

    public UpdateClassProfessorUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;
        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var professorClassMapper = new ClassProfessorMapper();

        var reader = new ClassProfessorsEFReader(_ctx, professorClassMapper);
        var updater = new ClassProfessorsEFUpdater(
            _ctx,
            professorClassMapper,
            new UpdateClassProfessorEFMapper()
        );

        var classProfessorQuerier = new ClassProfessorEFQuerier(
            _ctx,
            new ClassProfessorProjector(),
            10
        );

        _useCase = new UpdateClassProfessorUseCase(updater, reader, classProfessorQuerier);
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

    private async Task<ClassProfessor> SeedClassProfessor(
        string classId,
        ulong professorId,
        bool isOwner
    )
    {
        var relation = new ClassProfessor
        {
            ClassId = classId,
            ProfessorId = professorId,
            IsOwner = isOwner,
        };
        _ctx.ClassProfessors.Add(relation);
        await _ctx.SaveChangesAsync();
        return relation;
    }

    private static Executor AsExecutor(UserDomain value) =>
        new() { Id = value.Id, Role = value.Role };

    [Fact]
    public async Task ExecuteAsync_AsAdmin_UpdatesSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser();
        var cls = await SeedClass();
        var relation = await SeedClassProfessor(cls.Id, professor.Id, false);

        var dto = new ClassProfessorUpdateDTO
        {
            ClassId = cls.Id,
            UserId = professor.Id,
            IsOwner = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        await _ctx.Entry(relation).ReloadAsync();
        Assert.True(relation.IsOwner);
    }

    [Fact]
    public async Task ExecuteAsync_AsOwnerProfessorUpdatingSelf_UpdatesSuccessfully()
    {
        var professor = await SeedUser();
        var cls = await SeedClass();
        var relation = await SeedClassProfessor(cls.Id, professor.Id, true);

        var dto = new ClassProfessorUpdateDTO
        {
            ClassId = cls.Id,
            UserId = professor.Id,
            IsOwner = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(professor) }
        );

        Assert.True(result.IsOk);
        await _ctx.Entry(relation).ReloadAsync();
        Assert.True(relation.IsOwner);
    }

    [Fact]
    public async Task ExecuteAsync_AsNonOwnerProfessorUpdatingSelf_ReturnsError()
    {
        var professor = await SeedUser();
        var cls = await SeedClass();
        await SeedClassProfessor(cls.Id, professor.Id, false);

        var dto = new ClassProfessorUpdateDTO
        {
            ClassId = cls.Id,
            UserId = professor.Id,
            IsOwner = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(professor) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AsProfessorUpdatingOther_ReturnsUnauthorized()
    {
        var professorExecutor = await SeedUser();
        var professorToUpdate = await SeedUser();
        var cls = await SeedClass();
        await SeedClassProfessor(cls.Id, professorExecutor.Id, false);
        await SeedClassProfessor(cls.Id, professorToUpdate.Id, false);

        var dto = new ClassProfessorUpdateDTO
        {
            ClassId = cls.Id,
            UserId = professorToUpdate.Id,
            IsOwner = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(professorExecutor) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentRelation_ReturnsNotFoundError()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var dto = new ClassProfessorUpdateDTO
        {
            ClassId = "non-existent",
            UserId = 999,
            IsOwner = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
