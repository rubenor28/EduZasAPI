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
        _conn = new SqliteConnection("Data Source=:memory:");
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

    private async Task<User> SeedUser(UserType role, ulong userId = 1)
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

    [Fact]
    public async Task ExecuteAsync_WithValidProfessor_AddsRelationSuccessfully()
    {
        var professor = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass();
        var dto = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = professor.UserId },
            IsOwner = false,
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
        var relation = await _ctx.ClassProfessors.FindAsync(cls.ClassId, professor.UserId);
        Assert.NotNull(relation);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidAdmin_AddsRelationSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var cls = await SeedClass();
        var dto = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = admin.UserId },
            IsOwner = true,
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
        var relation = await _ctx.ClassProfessors.FindAsync(cls.ClassId, admin.UserId);
        Assert.NotNull(relation);
        Assert.True(relation.IsOwner);
    }

    [Fact]
    public async Task ExecuteAsync_WithStudent_ReturnsUnauthorizedError()
    {
        var student = await SeedUser(UserType.STUDENT);
        var cls = await SeedClass();
        var dto = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = student.UserId },
            IsOwner = false,
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ReturnsInputError()
    {
        var cls = await SeedClass();
        var dto = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = 999 },
            IsOwner = false,
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
        var professor = await SeedUser(UserType.PROFESSOR);
        var dto = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = "NON-EXISTENT", UserId = professor.UserId },
            IsOwner = false,
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
        var professor = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass();
        var dto = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = professor.UserId },
            IsOwner = false,
        };
        await _useCase.ExecuteAsync(dto); // First time should be ok

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        Assert.IsType<InputError>(result.UnwrapErr());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
