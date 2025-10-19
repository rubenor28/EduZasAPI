
using Application.DTOs.Classes;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.UseCases.ClassStudents;
using Domain.Enums;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DAOs.ClassStudents;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.ClassStudents;

public class UnenrollClassUseCaseTest : IDisposable
{
    private readonly UnenrollClassUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public UnenrollClassUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var userMapper = new UserEFMapper();
        var classMapper = new ClassEFMapper();
        var studentClassMapper = new StudentClassEFMapper();
        var professorClassMapper = new ProfessorClassEFMapper();

        var userReader = new UserEFReader(_ctx, userMapper);
        var classReader = new ClassEFReader(_ctx, classMapper);
        var studentDeleter = new ClassStudentsEFDeleter(_ctx, studentClassMapper);
        var professorReader = new ClassProfessorsEFReader(_ctx, professorClassMapper);
        var studentReader = new ClassStudentsEFReader(_ctx, studentClassMapper);

        _useCase = new UnenrollClassUseCase(
            userReader,
            classReader,
            studentDeleter,
            professorReader,
            studentReader
        );
    }

    private async Task<User> SeedUser(UserType role, ulong userId)
    {
        var user = new User
        {
            UserId = userId,
            Email = $"user{userId}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)role,
        };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return user;
    }

    private async Task<Class> SeedClass(string classId = "TEST-CLASS")
    {
        var cls = new Class { ClassId = classId, ClassName = "Test Class" };
        _ctx.Classes.Add(cls);
        await _ctx.SaveChangesAsync();
        return cls;
    }

    private async Task SeedStudentRelation(string classId, ulong studentId)
    {
        _ctx.ClassStudents.Add(new ClassStudent { ClassId = classId, StudentId = studentId, Hidden = false });
        await _ctx.SaveChangesAsync();
    }

    private async Task SeedProfessorRelation(string classId, ulong professorId, bool isOwner)
    {
        _ctx.ClassProfessors.Add(new ClassProfessor { ClassId = classId, ProfessorId = professorId, IsOwner = isOwner });
        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task ExecuteAsync_StudentUnenrollsThemselves_Succeeds()
    {
        var student = await SeedUser(UserType.STUDENT, 1);
        var cls = await SeedClass();
        await SeedStudentRelation(cls.ClassId, student.UserId);

        var dto = new UnenrollClassDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = student.UserId },
            Executor = new() { Id = student.UserId, Role = UserType.STUDENT }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
        var relation = await _ctx.ClassStudents.FindAsync(cls.ClassId, student.UserId);
        Assert.Null(relation);
    }

    [Fact]
    public async Task ExecuteAsync_AdminUnenrollsStudent_Succeeds()
    {
        var admin = await SeedUser(UserType.ADMIN, 1);
        var student = await SeedUser(UserType.STUDENT, 2);
        var cls = await SeedClass();
        await SeedStudentRelation(cls.ClassId, student.UserId);

        var dto = new UnenrollClassDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = student.UserId },
            Executor = new() { Id = admin.UserId, Role = UserType.ADMIN }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_OwnerUnenrollsStudent_Succeeds()
    {
        var owner = await SeedUser(UserType.PROFESSOR, 1);
        var student = await SeedUser(UserType.STUDENT, 2);
        var cls = await SeedClass();
        await SeedProfessorRelation(cls.ClassId, owner.UserId, true);
        await SeedStudentRelation(cls.ClassId, student.UserId);

        var dto = new UnenrollClassDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = student.UserId },
            Executor = new() { Id = owner.UserId, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_NonOwnerProfessorUnenrolls_ReturnsUnauthorized()
    {
        var nonOwner = await SeedUser(UserType.PROFESSOR, 1);
        var student = await SeedUser(UserType.STUDENT, 2);
        var cls = await SeedClass();
        await SeedProfessorRelation(cls.ClassId, nonOwner.UserId, false);
        await SeedStudentRelation(cls.ClassId, student.UserId);

        var dto = new UnenrollClassDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = student.UserId },
            Executor = new() { Id = nonOwner.UserId, Role = UserType.PROFESSOR }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AnotherStudentUnenrolls_ReturnsUnauthorized()
    {
        var student1 = await SeedUser(UserType.STUDENT, 1);
        var student2 = await SeedUser(UserType.STUDENT, 2);
        var cls = await SeedClass();
        await SeedStudentRelation(cls.ClassId, student2.UserId);

        var dto = new UnenrollClassDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = student2.UserId },
            Executor = new() { Id = student1.UserId, Role = UserType.STUDENT }
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_RelationNotFound_ReturnsNotFoundError()
    {
        var student = await SeedUser(UserType.STUDENT, 1);
        var cls = await SeedClass();
        // No relation seeded

        var dto = new UnenrollClassDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = student.UserId },
            Executor = new() { Id = student.UserId, Role = UserType.STUDENT }
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
