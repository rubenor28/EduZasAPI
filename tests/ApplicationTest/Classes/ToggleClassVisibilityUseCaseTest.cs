using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.UseCases.ClassStudents;
using Domain.Enums;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassStudents;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Classes;

public class ToggleClassVisibilityUseCaseTest : IDisposable
{
    private readonly ToggleClassVisibilityUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public ToggleClassVisibilityUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var studentClassMapper = new StudentClassEFMapper();
        var classMapper = new ClassEFMapper();
        var userMapper = new UserEFMapper();

        var studentClassUpdater = new ClassStudentsEFUpdater(
            _ctx,
            studentClassMapper,
            studentClassMapper
        );

        var classReader = new ClassEFReader(_ctx, classMapper);
        var userReader = new UserEFReader(_ctx, userMapper);
        var studentRelationReader = new ClassStudentsEFReader(_ctx, studentClassMapper);

        _useCase = new ToggleClassVisibilityUseCase(
            studentClassUpdater,
            classReader,
            userReader,
            studentRelationReader
        );
    }

    private async Task<User> SeedUser(ulong userId = 1)
    {
        var user = new User
        {
            UserId = userId,
            Email = $"student{userId}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)UserType.STUDENT,
        };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return user;
    }

    private async Task<Class> SeedClass()
    {
        var cls = new Class { ClassId = "TEST-CLASS", ClassName = "Test Class" };
        _ctx.Classes.Add(cls);
        await _ctx.SaveChangesAsync();
        return cls;
    }

    private async Task<ClassStudent> SeedRelation(string classId, ulong studentId, bool isHidden)
    {
        var relation = new ClassStudent
        {
            ClassId = classId,
            StudentId = studentId,
            Hidden = isHidden,
        };
        _ctx.ClassStudents.Add(relation);
        await _ctx.SaveChangesAsync();
        return relation;
    }

    [Fact]
    public async Task ExecuteAsync_WhenClassIsVisible_HidesClassSuccessfully()
    {
        var student = await SeedUser();
        var cls = await SeedClass();
        await SeedRelation(cls.ClassId, student.UserId, false); // Initially visible

        var toggleDto = new ToggleClassVisibilityDTO
        {
            ClassId = cls.ClassId,
            Executor = new() { Id = student.UserId, Role = UserType.STUDENT },
        };

        var result = await _useCase.ExecuteAsync(toggleDto);

        Assert.True(result.IsOk);

        var updatedRelation = await _ctx.ClassStudents.FindAsync(cls.ClassId, student.UserId);
        Assert.NotNull(updatedRelation);
        Assert.True(updatedRelation.Hidden);
    }

    [Fact]
    public async Task ExecuteAsync_WhenClassIsHidden_ShowsClassSuccessfully()
    {
        var student = await SeedUser();
        var cls = await SeedClass();
        await SeedRelation(cls.ClassId, student.UserId, true); // Initially hidden

        var toggleDto = new ToggleClassVisibilityDTO
        {
            ClassId = cls.ClassId,
            Executor = new() { Id = student.UserId, Role = UserType.STUDENT },
        };

        var result = await _useCase.ExecuteAsync(toggleDto);

        Assert.True(result.IsOk);
        var updatedRelation = await _ctx.ClassStudents.FindAsync(cls.ClassId, student.UserId);
        Assert.NotNull(updatedRelation);
        Assert.False(updatedRelation.Hidden);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClass_ReturnsInputError()
    {
        var student = await SeedUser();
        var toggleDto = new ToggleClassVisibilityDTO
        {
            ClassId = "NON-EXISTENT",
            Executor = new() { Id = student.UserId, Role = UserType.STUDENT },
        };

        var result = await _useCase.ExecuteAsync(toggleDto);

        Assert.True(result.IsErr);
        var error = result.UnwrapErr();
        Assert.IsType<InputError>(error);
        Assert.Contains(((InputError)error).Errors, e => e.Field == "classId");
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ReturnsInputError()
    {
        var cls = await SeedClass();
        var toggleDto = new ToggleClassVisibilityDTO
        {
            ClassId = cls.ClassId,
            Executor = new() { Id = 999, Role = UserType.STUDENT }, // Non-existent user
        };

        var result = await _useCase.ExecuteAsync(toggleDto);

        Assert.True(result.IsErr);
        var error = result.UnwrapErr();
        Assert.NotNull(error);
        Assert.IsType<InputError>(error);
        Assert.Contains(((InputError)error).Errors, e => e.Field == "userId");
    }

    [Fact]
    public async Task ExecuteAsync_WhenRelationDoesNotExist_ReturnsNotFoundError()
    {
        var student = await SeedUser();
        var cls = await SeedClass();
        // No relation seeded

        var toggleDto = new ToggleClassVisibilityDTO
        {
            ClassId = cls.ClassId,
            Executor = new() { Id = student.UserId, Role = UserType.STUDENT },
        };

        var result = await _useCase.ExecuteAsync(toggleDto);

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
