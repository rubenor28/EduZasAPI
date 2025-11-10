using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.UseCases.ClassStudents;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassStudents;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Classes;

public class ToggleClassVisibilityUseCaseTest : IDisposable
{
    private readonly ToggleClassVisibilityUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly UserEFMapper _userMapper;
    private readonly ClassEFMapper _classMapper;

    private readonly Random _rdm = new();

    public ToggleClassVisibilityUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var studentClassMapper = new StudentClassEFMapper();
        _classMapper = new();

        var roleMapper = new UserTypeMapper();
        _userMapper = new(roleMapper, roleMapper);

        var studentClassUpdater = new ClassStudentsEFUpdater(
            _ctx,
            studentClassMapper,
            studentClassMapper
        );

        var classReader = new ClassEFReader(_ctx, _classMapper);
        var userReader = new UserEFReader(_ctx, _userMapper);
        var studentRelationReader = new ClassStudentsEFReader(_ctx, studentClassMapper);

        _useCase = new ToggleClassVisibilityUseCase(
            studentClassUpdater,
            classReader,
            userReader,
            studentRelationReader
        );
    }

    private async Task<UserDomain> SeedUser(UserType userType = UserType.STUDENT)
    {
        var userId = (ulong)_rdm.Next(1, 100_000);
        var user = new User
        {
            UserId = userId,
            Email = $"student{userId}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)userType,
        };

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return _userMapper.Map(user);
    }

    private async Task SeedEnroll(ulong userId, string classId)
    {
        var student = new ClassStudent
        {
            ClassId = classId,
            StudentId = userId,
            Hidden = false,
        };

        _ctx.ClassStudents.Add(student);
        await _ctx.SaveChangesAsync();
    }

    private async Task<ClassDomain> SeedClass()
    {
        var id = _rdm.Next(1, 100_000);
        var cls = new Class { ClassId = $"TEST-CLASS-{id}", ClassName = "Test Class" };
        _ctx.Classes.Add(cls);
        await _ctx.SaveChangesAsync();
        return _classMapper.Map(cls);
    }

    private async Task<ClassStudent> SeedRelation(
        string classId,
        ulong studentId,
        bool isHidden = false
    )
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

    private static Executor AsExecutor(UserDomain user) => new() { Id = user.Id, Role = user.Role };

    [Fact]
    public async Task ExecuteAsync_WhenClassIsVisible_HidesClassSuccessfully()
    {
        var student = await SeedUser();
        var cls = await SeedClass();
        await SeedRelation(cls.Id, student.Id);

        var toggleDto = new ToggleClassVisibilityDTO
        {
            ClassId = cls.Id,
            UserId = student.Id,
            Executor = AsExecutor(student),
        };

        var result = await _useCase.ExecuteAsync(toggleDto);

        Assert.True(result.IsOk);

        var updatedRelation = await _ctx.ClassStudents.FindAsync(cls.Id, student.Id);
        Assert.NotNull(updatedRelation);
        Assert.True(updatedRelation.Hidden);
    }

    [Fact]
    public async Task ExecuteAsync_WhenClassIsHidden_ShowsClassSuccessfully()
    {
        var student = await SeedUser();
        var cls = await SeedClass();
        await SeedRelation(cls.Id, student.Id, true); // Initially hidden

        var toggleDto = new ToggleClassVisibilityDTO
        {
            ClassId = cls.Id,
            UserId = student.Id,
            Executor = AsExecutor(student),
        };

        var result = await _useCase.ExecuteAsync(toggleDto);

        Assert.True(result.IsOk);
        var updatedRelation = await _ctx.ClassStudents.FindAsync(cls.Id, student.Id);
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
            UserId = student.Id,
            Executor = AsExecutor(student),
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
        var admin = await SeedUser(UserType.ADMIN);
        var cls = await SeedClass();
        var toggleDto = new ToggleClassVisibilityDTO
        {
            ClassId = cls.Id,
            UserId = 999, // Non-existent user
            Executor = AsExecutor(admin),
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
            ClassId = cls.Id,
            UserId = student.Id,
            Executor = AsExecutor(student),
        };

        var result = await _useCase.ExecuteAsync(toggleDto);

        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AsAdmin_TogglesSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var student = await SeedUser();
        var cls = await SeedClass();
        await SeedRelation(cls.Id, student.Id, false);

        var toggleDto = new ToggleClassVisibilityDTO
        {
            ClassId = cls.Id,
            UserId = student.Id,
            Executor = new() { Id = admin.Id, Role = UserType.ADMIN },
        };

        var result = await _useCase.ExecuteAsync(toggleDto);

        Assert.True(result.IsOk);

        var updatedRelation = await _ctx.ClassStudents.FindAsync(cls.Id, student.Id);
        Assert.NotNull(updatedRelation);
        Assert.True(updatedRelation.Hidden);
    }

    [Fact]
    public async Task ExecuteAsync_AsProfessor_ForStudent_ReturnsUnauthorized()
    {
        var professor = await SeedUser(UserType.PROFESSOR);
        var student = await SeedUser();
        var cls = await SeedClass();
        await SeedRelation(cls.Id, student.Id, false);

        var toggleDto = new ToggleClassVisibilityDTO
        {
            ClassId = cls.Id,
            UserId = student.Id,
            Executor = new() { Id = professor.Id, Role = UserType.PROFESSOR },
        };

        var result = await _useCase.ExecuteAsync(toggleDto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AsStudent_ForAnotherStudent_ReturnsUnauthorized()
    {
        var student1 = await SeedUser();
        var student2 = await SeedUser();
        var cls = await SeedClass();
        await SeedRelation(cls.Id, student2.Id, false);

        var toggleDto = new ToggleClassVisibilityDTO
        {
            ClassId = cls.Id,
            UserId = student2.Id,
            Executor = new() { Id = student1.Id, Role = UserType.STUDENT },
        };

        var result = await _useCase.ExecuteAsync(toggleDto);

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
