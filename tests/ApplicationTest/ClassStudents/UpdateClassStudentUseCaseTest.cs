using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.UseCases.ClassStudents;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DAOs.ClassStudents;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.ClassStudents;

public class UpdateClassStudentUseCaseTest : IDisposable
{
    private readonly UpdateClassStudentUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;
    private readonly UserEFMapper _userMapper;
    private readonly ClassEFMapper _classMapper = new();
    private readonly ClassStudentEFMapper _classStudentMapper = new();
    private readonly Random _rdm = new();

    public UpdateClassStudentUseCaseTest()
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

        var userReader = new UserEFReader(_ctx, _userMapper);
        var classReader = new ClassEFReader(_ctx, _classMapper);
        var studentReader = new ClassStudentsEFReader(_ctx, _classStudentMapper);
        var professorReader = new ClassProfessorsEFReader(_ctx, professorClassMapper);
        var updater = new ClassStudentsEFUpdater(_ctx, _classStudentMapper, _classStudentMapper);

        _useCase = new UpdateClassStudentUseCase(updater, studentReader, professorReader, null);
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.STUDENT)
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

    private async Task<ClassStudent> SeedClassStudent(string classId, ulong studentId, bool hidden = false)
    {
        var relation = new ClassStudent { ClassId = classId, StudentId = studentId, Hidden = hidden };
        _ctx.ClassStudents.Add(relation);
        await _ctx.SaveChangesAsync();
        return relation;
    }

    private async Task SeedClassProfessor(string classId, ulong professorId)
    {
        var relation = new ClassProfessor { ClassId = classId, ProfessorId = professorId, IsOwner = false };
        _ctx.ClassProfessors.Add(relation);
        await _ctx.SaveChangesAsync();
    }

    private static Executor AsExecutor(UserDomain value) => new() { Id = value.Id, Role = value.Role };

    [Fact]
    public async Task ExecuteAsync_AsAdmin_UpdatesSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var student = await SeedUser();
        var cls = await SeedClass();
        var relation = await SeedClassStudent(cls.Id, student.Id, false);

        var dto = new ClassStudentUpdateDTO
        {
            Id = new() { ClassId = cls.Id, UserId = student.Id },
            Hidden = true,
            Executor = AsExecutor(admin)
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
        await _ctx.Entry(relation).ReloadAsync();
        Assert.True(relation.Hidden);
    }

    [Fact]
    public async Task ExecuteAsync_AsAuthorizedProfessor_UpdatesSuccessfully()
    {
        var professor = await SeedUser(UserType.PROFESSOR);
        var student = await SeedUser();
        var cls = await SeedClass();
        await SeedClassProfessor(cls.Id, professor.Id);
        var relation = await SeedClassStudent(cls.Id, student.Id, false);

        var dto = new ClassStudentUpdateDTO
        {
            Id = new() { ClassId = cls.Id, UserId = student.Id },
            Hidden = true,
            Executor = AsExecutor(professor)
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
        await _ctx.Entry(relation).ReloadAsync();
        Assert.True(relation.Hidden);
    }

    [Fact]
    public async Task ExecuteAsync_AsUnauthorizedProfessor_ReturnsUnauthorizedError()
    {
        var professor = await SeedUser(UserType.PROFESSOR);
        var student = await SeedUser();
        var classA = await SeedClass();
        var classB = await SeedClass();
        await SeedClassProfessor(classB.Id, professor.Id); // Professor of another class
        await SeedClassStudent(classA.Id, student.Id);

        var dto = new ClassStudentUpdateDTO
        {
            Id = new() { ClassId = classA.Id, UserId = student.Id },
            Hidden = true,
            Executor = AsExecutor(professor)
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AsStudent_ReturnsUnauthorizedError()
    {
        var studentExecutor = await SeedUser();
        var studentToUpdate = await SeedUser();
        var cls = await SeedClass();
        await SeedClassStudent(cls.Id, studentToUpdate.Id);

        var dto = new ClassStudentUpdateDTO
        {
            Id = new() { ClassId = cls.Id, UserId = studentToUpdate.Id },
            Hidden = true,
            Executor = AsExecutor(studentExecutor)
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentRelation_ReturnsNotFoundError()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var dto = new ClassStudentUpdateDTO
        {
            Id = new() { ClassId = "non-existent", UserId = 999 },
            Hidden = true,
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
