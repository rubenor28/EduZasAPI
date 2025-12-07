using Application.DTOs;
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

public class AddClassStudentUseCaseTest : IDisposable
{
    private readonly AddClassStudentUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly UserMapper _userMapper = new();
    private readonly ClassMapper _classMapper = new();

    private readonly Random _rdm = new();

    public AddClassStudentUseCaseTest()
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
        var studentReader = new ClassStudentsEFReader(_ctx, studentClassMapper);
        var professorReader = new ClassProfessorsEFReader(_ctx, professorClassMapper);
        var creator = new ClassStudentEFCreator(
            _ctx,
            studentClassMapper,
            new NewClassStudentEFMapper()
        );

        _useCase = new AddClassStudentUseCase(
            creator,
            userReader,
            classReader,
            studentReader,
            professorReader
        );
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.STUDENT)
    {
        var id = (ulong)_rdm.NextInt64(1, 100_000);

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
        var id = (ulong)_rdm.NextInt64(1, 100_000);
        var cls = new Class { ClassId = $"class-{id}", ClassName = "Test Class" };
        _ctx.Classes.Add(cls);
        await _ctx.SaveChangesAsync();
        return _classMapper.Map(cls);
    }

    public static Executor AsExecutor(UserDomain value) =>
        new() { Id = value.Id, Role = value.Role };

    [Fact]
    public async Task ExecuteAsync_WithValidData_EnrollsSuccessfully()
    {
        var user = await SeedUser();
        var cls = await SeedClass();
        var adminUser = await SeedUser(UserType.ADMIN);

        var dto = new UserClassRelationId { ClassId = cls.Id, UserId = user.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(adminUser) }
        );

        Assert.True(result.IsOk);
        var relation = await _ctx.ClassStudents.FindAsync(cls.Id, user.Id);
        Assert.NotNull(relation);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAlreadyEnrolled_ReturnsInputError()
    {
        var user = await SeedUser();
        var cls = await SeedClass();
        var adminUser = await SeedUser(UserType.ADMIN);

        var dto = new UserClassRelationId { ClassId = cls.Id, UserId = user.Id };

        var data = new UserActionDTO<UserClassRelationId>
        {
            Data = dto,
            Executor = AsExecutor(adminUser),
        };

        await _useCase.ExecuteAsync(data); // Enroll first time

        var result = await _useCase.ExecuteAsync(data); // Attempt to enroll again

        Assert.True(result.IsErr);
        Assert.IsType<Conflict>(result.UnwrapErr());
        var err = (Conflict)result.UnwrapErr();
        Assert.Equal("El recurso ya existe", err.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClass_ReturnsInputError()
    {
        var user = await SeedUser();
        var dto = new UserClassRelationId { ClassId = "NON-EXISTENT", UserId = user.Id };

        var result = await _useCase.ExecuteAsync(new() { Data = dto, Executor = AsExecutor(user) });

        Assert.True(result.IsErr);
        Assert.IsType<InputError>(result.UnwrapErr());
        var error = result.UnwrapErr();
        Assert.Contains(
            ((InputError)error).Errors,
            e => e.Field == "classId" && e.Message == "Clase no encontrada"
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ReturnsInputError()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var cls = await SeedClass();
        var dto = new UserClassRelationId { ClassId = cls.Id, UserId = 999 };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsErr);
        var error = result.UnwrapErr() as InputError;
        Assert.NotNull(error);
        Assert.Contains(
            error.Errors,
            e => e.Field == "userId" && e.Message == "Usuario no encontrado"
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsProfessor_ReturnsInputError()
    {
        var professor = await SeedUser();
        var cls = await SeedClass();
        _ctx.ClassProfessors.Add(
            new ClassProfessor
            {
                ClassId = cls.Id,
                ProfessorId = professor.Id,
                IsOwner = false,
            }
        );
        await _ctx.SaveChangesAsync();

        var dto = new UserClassRelationId { ClassId = cls.Id, UserId = professor.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(professor) }
        );

        Assert.True(result.IsErr);
        var error = result.UnwrapErr() as InputError;
        Assert.NotNull(error);
        Assert.Contains(
            error.Errors,
            e => e.Message.Contains("El usuario ya es profesor de la clase")
        );
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
