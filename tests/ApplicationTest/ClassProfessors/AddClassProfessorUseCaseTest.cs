using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.UseCases.ClassProfessors;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.ClassProfessors;

public class AddClassProfessorUseCaseTest : IDisposable
{
    private readonly AddClassProfessorUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly UserMapper _userMapper = new();
    private readonly ClassMapper _classMapper = new();

    private readonly Random _random = new();

    public AddClassProfessorUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var userReader = new UserEFReader(_ctx, _userMapper);
        var classReader = new ClassEFReader(_ctx, _classMapper);

        var classProfessorMapper = new ClassProfessorMapper();

        var professorReader = new ClassProfessorsEFReader(_ctx, classProfessorMapper);

        var professorCreator = new ClassProfessorsEFCreator(
            _ctx,
            classProfessorMapper,
            new NewClassProfessorEFMapper()
        );

        _useCase = new AddClassProfessorUseCase(
            professorCreator,
            professorReader,
            userReader,
            classReader
        );
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.PROFESSOR)
    {
        var id = (ulong)_random.Next(1, 100_000);
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

    private async Task<ClassDomain> SeedClass()
    {
        var number = _random.Next(1, 100_000);
        var cls = new Class { ClassId = $"class-test-{number}", ClassName = "Test Class" };
        _ctx.Classes.Add(cls);
        await _ctx.SaveChangesAsync();
        return _classMapper.Map(cls);
    }

    private static Executor AsExecutor(UserDomain user) => new() { Id = user.Id, Role = user.Role };

    [Fact]
    public async Task ExecuteAsync_WithValidAdmin_AddsRelationSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var target = await SeedUser();
        var cls = await SeedClass();

        var dto = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = target.Id,
            IsOwner = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var relation = await _ctx.ClassProfessors.FindAsync(cls.Id, target.Id);
        Assert.NotNull(relation);
        Assert.True(relation.IsOwner);
    }

    [Fact]
    public async Task ExecuteAsync_WithProfessorOwner_AddsRelationSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var ownerProfessor = await SeedUser();
        var newProfessor = await SeedUser();
        var cls = await SeedClass();

        var bootstrapDto = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = ownerProfessor.Id,
            IsOwner = true,
        };
        var bootstrap = await _useCase.ExecuteAsync(
            new() { Data = bootstrapDto, Executor = AsExecutor(admin) }
        );
        Assert.True(bootstrap.IsOk);

        var dto = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = newProfessor.Id,
            IsOwner = false,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(ownerProfessor) }
        );
        Assert.True(result.IsOk);
        var relation = await _ctx.ClassProfessors.FindAsync(cls.Id, newProfessor.Id);
        Assert.NotNull(relation);
        Assert.False(relation.IsOwner);
    }

    [Fact]
    public async Task ExecuteAsync_WithStudent_ReturnsUnauthorizedError()
    {
        var studentExecutor = await SeedUser(UserType.STUDENT);
        var target = await SeedUser();
        var cls = await SeedClass();

        var dto = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = target.Id,
            IsOwner = false,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(studentExecutor) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ReturnsInputError()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var cls = await SeedClass();

        var dto = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = 999999,
            IsOwner = false,
        };

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
    public async Task ExecuteAsync_WithNonExistentClass_ReturnsInputError()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var target = await SeedUser();

        var dto = new NewClassProfessorDTO
        {
            ClassId = "NON-EXISTENT",
            UserId = target.Id,
            IsOwner = false,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsErr);
        var error = result.UnwrapErr() as InputError;
        Assert.NotNull(error);
        Assert.Contains(
            error.Errors,
            e => e.Field == "classId" && e.Message == "Clase no encontrada"
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenRelationAlreadyExists_ReturnsInputError()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var target = await SeedUser();
        var cls = await SeedClass();

        var dto = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = target.Id,
            IsOwner = false,
        };

        var first = await _useCase.ExecuteAsync(new() { Data = dto, Executor = AsExecutor(admin) });
        Assert.True(first.IsOk);

        var second = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        Assert.True(second.IsErr);
        Assert.IsType<AlreadyExistsError>(second.UnwrapErr());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
