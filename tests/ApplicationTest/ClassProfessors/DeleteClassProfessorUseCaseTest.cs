using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.UseCases.ClassProfessors;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.ClassProfessors;

public class DeleteClassProfessorUseCaseTest : IDisposable
{
    private readonly DeleteClassProfessorUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;
    private readonly UserEFMapper _userMapper;
    private readonly ClassEFMapper _classMapper = new();
    private readonly Random _rdm = new();

    public DeleteClassProfessorUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;
        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var roleMapper = new UserTypeMapper();
        _userMapper = new UserEFMapper(roleMapper, roleMapper);

        var professorClassMapper = new ClassProfessorEFMapper();

        var reader = new ClassProfessorsEFReader(_ctx, professorClassMapper);
        var deleter = new ClassProfessorsEFDeleter(_ctx, professorClassMapper);

        _useCase = new DeleteClassProfessorUseCase(deleter, reader, null);
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.PROFESSOR)
    {
        var id = (ulong)_rdm.NextInt64(1, 100_000);
        var user = new User { UserId = id, Email = $"user-{id}@test.com", FirstName = "test", FatherLastname = "test", Password = "test", Role = (uint)role };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return _userMapper.Map(user);
    }

    private async Task<ClassDomain> SeedClass()
    {
        var id = (ulong)_rdm.NextInt64(1, 100_000);
        var cls = new Class { ClassId = $"class-{id}", ClassName = "Test Class" };
        _ctx.Classes.Add(cls);
        await _ctx.SaveChangesAsync();
        return _classMapper.Map(cls);
    }

    private async Task SeedClassProfessor(string classId, ulong professorId, bool isOwner)
    {
        var relation = new ClassProfessor { ClassId = classId, ProfessorId = professorId, IsOwner = isOwner };
        _ctx.ClassProfessors.Add(relation);
        await _ctx.SaveChangesAsync();
    }

    private static Executor AsExecutor(UserDomain value) => new() { Id = value.Id, Role = value.Role };

    [Fact]
    public async Task ExecuteAsync_AsAdmin_DeletesSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professorToDelete = await SeedUser();
        var cls = await SeedClass();
        await SeedClassProfessor(cls.Id, professorToDelete.Id, false);

        var dto = new DeleteClassProfessorDTO
        {
            Id = new() { ClassId = cls.Id, UserId = professorToDelete.Id },
            Executor = AsExecutor(admin)
        };

        var result = await _useCase.ExecuteAsync(dto);
        var found = await _ctx.ClassProfessors.FindAsync(cls.Id, professorToDelete.Id);

        Assert.True(result.IsOk);
        Assert.Null(found);
    }

    [Fact]
    public async Task ExecuteAsync_AsOwnerProfessor_DeletesSuccessfully()
    {
        var ownerProfessor = await SeedUser();
        var professorToDelete = await SeedUser();
        var cls = await SeedClass();
        await SeedClassProfessor(cls.Id, ownerProfessor.Id, true);
        await SeedClassProfessor(cls.Id, professorToDelete.Id, false);

        var dto = new DeleteClassProfessorDTO
        {
            Id = new() { ClassId = cls.Id, UserId = professorToDelete.Id },
            Executor = AsExecutor(ownerProfessor)
        };

        var result = await _useCase.ExecuteAsync(dto);
        var found = await _ctx.ClassProfessors.FindAsync(cls.Id, professorToDelete.Id);

        Assert.True(result.IsOk);
        Assert.Null(found);
    }

    [Fact]
    public async Task ExecuteAsync_AsNonOwnerProfessor_ReturnsUnauthorized()
    {
        var nonOwnerProfessor = await SeedUser();
        var professorToDelete = await SeedUser();
        var cls = await SeedClass();
        await SeedClassProfessor(cls.Id, nonOwnerProfessor.Id, false);
        await SeedClassProfessor(cls.Id, professorToDelete.Id, false);

        var dto = new DeleteClassProfessorDTO
        {
            Id = new() { ClassId = cls.Id, UserId = professorToDelete.Id },
            Executor = AsExecutor(nonOwnerProfessor)
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AsStudent_ReturnsUnauthorized()
    {
        var student = await SeedUser(UserType.STUDENT);
        var professorToDelete = await SeedUser();
        var cls = await SeedClass();
        await SeedClassProfessor(cls.Id, professorToDelete.Id, false);

        var dto = new DeleteClassProfessorDTO
        {
            Id = new() { ClassId = cls.Id, UserId = professorToDelete.Id },
            Executor = AsExecutor(student)
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentRelation_ReturnsNotFoundError()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var cls = await SeedClass();

        var dto = new DeleteClassProfessorDTO
        {
            Id = new() { ClassId = cls.Id, UserId = 999 },
            Executor = AsExecutor(admin)
        };

        var result = await _useCase.ExecuteAsync(dto);

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
