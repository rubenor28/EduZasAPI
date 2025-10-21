using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.UseCases.ClassProfessors;
using Domain.Enums;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.ClassProfessors;

public class AddProfessorToClassUseCaseTest : IDisposable
{
    private readonly AddProfessorToClassUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public AddProfessorToClassUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var userMapper = new UserEFMapper();
        var classMapper = new ClassEFMapper();
        var classProfessorMapper = new ProfessorClassEFMapper();

        var classProfessorCreator = new ClassProfessorEFCreator(
            _ctx,
            classProfessorMapper,
            classProfessorMapper
        );

        var userReader = new UserEFReader(_ctx, userMapper);
        var classReader = new ClassEFReader(_ctx, classMapper);
        var professorClassRelationReader = new ClassProfessorsEFReader(_ctx, classProfessorMapper);

        _useCase = new AddProfessorToClassUseCase(
            classProfessorCreator,
            userReader,
            classReader,
            professorClassRelationReader
        );
    }

    private async Task<User> SeedUser(UserType role, ulong userId = 0)
    {
        var id = userId == 0 ? (ulong)new Random().Next(1000, 100000) : userId;
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
        return user;
    }

    private async Task<Class> SeedClass(string classId = "TEST-CLASS")
    {
        var cls = new Class { ClassId = classId, ClassName = "Test Class" };
        _ctx.Classes.Add(cls);
        await _ctx.SaveChangesAsync();
        return cls;
    }

    [Fact]
    public async Task ExecuteAsync_WithValidAdmin_AddsRelationSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var target = await SeedUser(UserType.PROFESSOR, 200);
        var cls = await SeedClass();

        var dto = new AddProfessorToClassDTO
        {
            ClassId = cls.ClassId,
            UserId = target.UserId,
            IsOwner = true,
            Executor = new Executor { Id = admin.UserId, Role = UserType.ADMIN },
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
        var relation = await _ctx.ClassProfessors.FindAsync(cls.ClassId, target.UserId);
        Assert.NotNull(relation);
        Assert.True(relation.IsOwner);
    }

    [Fact]
    public async Task ExecuteAsync_WithProfessorOwner_AddsRelationSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var ownerProfessor = await SeedUser(UserType.PROFESSOR, 300);
        var newProfessor = await SeedUser(UserType.PROFESSOR, 301);
        var cls = await SeedClass("CLASS-PROF");

        var bootstrapDto = new AddProfessorToClassDTO
        {
            ClassId = cls.ClassId,
            UserId = ownerProfessor.UserId,
            IsOwner = true,
            Executor = new Executor { Id = admin.UserId, Role = UserType.ADMIN },
        };
        var bootstrap = await _useCase.ExecuteAsync(bootstrapDto);
        Assert.True(bootstrap.IsOk);

        var dto = new AddProfessorToClassDTO
        {
            ClassId = cls.ClassId,
            UserId = newProfessor.UserId,
            IsOwner = false,
            Executor = new Executor { Id = ownerProfessor.UserId, Role = UserType.PROFESSOR },
        };

        var result = await _useCase.ExecuteAsync(dto);
        Assert.True(result.IsOk);
        var relation = await _ctx.ClassProfessors.FindAsync(cls.ClassId, newProfessor.UserId);
        Assert.NotNull(relation);
        Assert.False(relation.IsOwner);
    }

    [Fact]
    public async Task ExecuteAsync_WithStudent_ReturnsUnauthorizedError()
    {
        var studentExecutor = await SeedUser(UserType.STUDENT);
        var target = await SeedUser(UserType.PROFESSOR, 400);
        var cls = await SeedClass("CLASS-STU");

        var dto = new AddProfessorToClassDTO
        {
            ClassId = cls.ClassId,
            UserId = target.UserId,
            IsOwner = false,
            Executor = new Executor { Id = studentExecutor.UserId, Role = UserType.STUDENT },
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ReturnsInputError()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var cls = await SeedClass("CLASS-NO-USER");

        var dto = new AddProfessorToClassDTO
        {
            ClassId = cls.ClassId,
            UserId = 999999,
            IsOwner = false,
            Executor = new Executor { Id = admin.UserId, Role = UserType.ADMIN },
        };

        var result = await _useCase.ExecuteAsync(dto);

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
        var target = await SeedUser(UserType.PROFESSOR, 500);

        var dto = new AddProfessorToClassDTO
        {
            ClassId = "NON-EXISTENT",
            UserId = target.UserId,
            IsOwner = false,
            Executor = new Executor { Id = admin.UserId, Role = UserType.ADMIN },
        };

        var result = await _useCase.ExecuteAsync(dto);

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
        var target = await SeedUser(UserType.PROFESSOR, 600);
        var cls = await SeedClass("CLASS-DUP");

        var dto = new AddProfessorToClassDTO
        {
            ClassId = cls.ClassId,
            UserId = target.UserId,
            IsOwner = false,
            Executor = new Executor { Id = admin.UserId, Role = UserType.ADMIN },
        };

        var first = await _useCase.ExecuteAsync(dto);
        Assert.True(first.IsOk);

        var second = await _useCase.ExecuteAsync(dto);
        Assert.True(second.IsOk);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
