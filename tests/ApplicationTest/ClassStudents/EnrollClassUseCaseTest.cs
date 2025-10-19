
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

public class EnrollClassUseCaseTest : IDisposable
{
    private readonly EnrollClassUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public EnrollClassUseCaseTest()
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
        var studentReader = new ClassStudentsEFReader(_ctx, studentClassMapper);
        var professorReader = new ClassProfessorsEFReader(_ctx, professorClassMapper);
        var creator = new ClassStudentEFCreator(_ctx, studentClassMapper, studentClassMapper);

        _useCase = new EnrollClassUseCase(
            userReader,
            classReader,
            studentReader,
            professorReader,
            creator
        );
    }

    private async Task<User> SeedUser(ulong userId = 1)
    {
        var user = new User
        {
            UserId = userId,
            Email = $"user{userId}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)UserType.STUDENT,
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
    public async Task ExecuteAsync_WithValidData_EnrollsSuccessfully()
    {
        var user = await SeedUser();
        var cls = await SeedClass();
        var dto = new StudentClassRelationDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = user.UserId },
            Hidden = false
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsOk);
        var relation = await _ctx.ClassStudents.FindAsync(cls.ClassId, user.UserId);
        Assert.NotNull(relation);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAlreadyEnrolled_ReturnsInputError()
    {
        var user = await SeedUser();
        var cls = await SeedClass();
        var dto = new StudentClassRelationDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = user.UserId },
            Hidden = false
        };
        await _useCase.ExecuteAsync(dto); // Enroll first time

        var result = await _useCase.ExecuteAsync(dto); // Attempt to enroll again

        Assert.True(result.IsErr);
        var error = result.UnwrapErr() as InputError;
        Assert.NotNull(error);
        Assert.Contains(error.Errors, e => e.Message.Contains("El usuario ya se encuentra inscrito"));
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClass_ReturnsInputError()
    {
        var user = await SeedUser();
        var dto = new StudentClassRelationDTO
        {
            Id = new() { ClassId = "NON-EXISTENT", UserId = user.UserId },
            Hidden = false
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        var error = result.UnwrapErr() as InputError;
        Assert.NotNull(error);
        Assert.Contains(error.Errors, e => e.Field == "classId" && e.Message == "Clase no encontrada");
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ReturnsInputError()
    {
        var cls = await SeedClass();
        var dto = new StudentClassRelationDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = 999 },
            Hidden = false
        };

        var result = await _useCase.ExecuteAsync(dto);

        Assert.True(result.IsErr);
        var error = result.UnwrapErr() as InputError;
        Assert.NotNull(error);
        Assert.Contains(error.Errors, e => e.Field == "userId" && e.Message == "Usuario no encontrado");
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserIsProfessor_ReturnsInputError()
    {
        var professor = await SeedUser();
        var cls = await SeedClass();
        _ctx.ClassProfessors.Add(new ClassProfessor { ClassId = cls.ClassId, ProfessorId = professor.UserId, IsOwner = false });
        await _ctx.SaveChangesAsync();

        var dto = new StudentClassRelationDTO
        {
            Id = new() { ClassId = cls.ClassId, UserId = professor.UserId },
            Hidden = false
        };

        var result = await _useCase.ExecuteAsync(dto);



        Assert.True(result.IsErr);
        var error = result.UnwrapErr() as InputError;
        Assert.NotNull(error);
        Assert.Contains(error.Errors, e => e.Message.Contains("El usuario ya es profesor de la clase"));
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
