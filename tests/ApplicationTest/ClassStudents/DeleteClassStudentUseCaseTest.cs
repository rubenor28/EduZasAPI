using Application.DTOs.Common;
using Application.UseCases.ClassStudents;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DAOs.ClassStudents;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;
using EntityFramework.InterfaceAdapters.Mappers.ClassStudents;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.ClassStudents;

public class DeleteClassStudentUseCaseTest : IDisposable
{
    private readonly DeleteClassStudentUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly Random _rdm = new();

    private readonly ClassMapper _classMapper = new();
    private readonly UserMapper _userMapper = new();

    public DeleteClassStudentUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var studentClassMapper = new ClassStudentMapper();
        var professorClassMapper = new ClassProfessorMapper();

        var userReader = new UserEFReader(_ctx, _userMapper);
        var classReader = new ClassEFReader(_ctx, _classMapper);
        var studentDeleter = new ClassStudentsEFDeleter(_ctx, studentClassMapper);
        var professorReader = new ClassProfessorsEFReader(_ctx, professorClassMapper);
        var studentReader = new ClassStudentsEFReader(_ctx, studentClassMapper);

        _useCase = new DeleteClassStudentUseCase(
            studentDeleter,
            studentReader,
            userReader,
            classReader,
            professorReader
        );
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.STUDENT)
    {
        var id = (ulong)_rdm.NextInt64(1, 100_000);
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

    private static Executor AsExecutor(UserDomain u) => new() { Id = u.Id, Role = u.Role };

    private async Task<ClassDomain> SeedClass()
    {
        var id = _rdm.NextInt64(1, 10_000);
        var cls = new Class { ClassId = $"class-{id}", ClassName = "Test Class" };
        _ctx.Classes.Add(cls);
        await _ctx.SaveChangesAsync();
        return _classMapper.Map(cls);
    }

    private async Task SeedStudentRelation(string classId, ulong studentId)
    {
        _ctx.ClassStudents.Add(
            new ClassStudent
            {
                ClassId = classId,
                StudentId = studentId,
                Hidden = false,
            }
        );
        await _ctx.SaveChangesAsync();
    }

    private async Task SeedProfessorRelation(string classId, ulong professorId, bool isOwner = true)
    {
        _ctx.ClassProfessors.Add(
            new ClassProfessor
            {
                ClassId = classId,
                ProfessorId = professorId,
                IsOwner = isOwner,
            }
        );
        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task ExecuteAsync_StudentUnenrollsThemselves_Succeeds()
    {
        var student = await SeedUser();
        var cls = await SeedClass();
        await SeedStudentRelation(cls.Id, student.Id);

        var dto = new UserClassRelationId { ClassId = cls.Id, UserId = student.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(student) }
        );

        Assert.True(result.IsOk);
        var relation = await _ctx.ClassStudents.FindAsync(cls.Id, student.Id);
        Assert.Null(relation);
    }

    [Fact]
    public async Task ExecuteAsync_AdminUnenrollsStudent_Succeeds()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var student = await SeedUser();
        var cls = await SeedClass();
        await SeedStudentRelation(cls.Id, student.Id);

        var dto = new UserClassRelationId { ClassId = cls.Id, UserId = student.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_OwnerUnenrollsStudent_Succeeds()
    {
        var owner = await SeedUser(UserType.PROFESSOR);
        var student = await SeedUser();
        var cls = await SeedClass();
        await SeedProfessorRelation(cls.Id, owner.Id);
        await SeedStudentRelation(cls.Id, student.Id);

        var dto = new UserClassRelationId { ClassId = cls.Id, UserId = student.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(owner) }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_NonOwnerProfessorUnenrolls_ReturnsUnauthorized()
    {
        var nonOwner = await SeedUser(UserType.PROFESSOR);
        var student = await SeedUser();
        var cls = await SeedClass();
        await SeedProfessorRelation(cls.Id, nonOwner.Id, false);
        await SeedStudentRelation(cls.Id, student.Id);

        var dto = new UserClassRelationId { ClassId = cls.Id, UserId = student.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(nonOwner) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AnotherStudentUnenrolls_ReturnsUnauthorized()
    {
        var student1 = await SeedUser();
        var student2 = await SeedUser();
        var cls = await SeedClass();
        await SeedStudentRelation(cls.Id, student2.Id);

        var dto = new UserClassRelationId { ClassId = cls.Id, UserId = student2.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(student1) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_RelationNotFound_ReturnsNotFoundError()
    {
        var student = await SeedUser();
        var cls = await SeedClass();
        // No relation seeded

        var dto = new UserClassRelationId { ClassId = cls.Id, UserId = student.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(student) }
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
