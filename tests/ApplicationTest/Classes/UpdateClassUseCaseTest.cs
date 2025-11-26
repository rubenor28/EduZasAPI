using Application.DTOs.Classes;
using Application.DTOs.Common;
using Application.UseCases.Classes;
using Domain.Enums;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;
using FluentValidationProj.Application.Services.Classes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Classes;

public class UpdateClassUseCaseTest : IDisposable
{
    private readonly UpdateClassUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public UpdateClassUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var classMapper = new ClassProjector();
        var professorRelationMapper = new ClassProfessorProjector();

        var classUpdateValidator = new ClassUpdateFluentValidator();

        var classReader = new ClassEFReader(_ctx, classMapper);
        var professorRelationReader = new ClassProfessorsEFReader(_ctx, professorRelationMapper);
        var classUpdater = new ClassEFUpdater(_ctx, classMapper, new UpdateClassEFMapper());

        _useCase = new UpdateClassUseCase(
            classUpdater,
            classReader,
            classUpdateValidator,
            professorRelationReader
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
        var cls = new Class
        {
            ClassId = classId,
            ClassName = "Old Name",
            Active = true,
            Color = "#ffffff",
            Section = "Section A",
            Subject = "Mathematics",
        };
        _ctx.Classes.Add(cls);
        await _ctx.SaveChangesAsync();
        return cls;
    }

    private async Task SeedRelation(string classId, ulong professorId, bool isOwner)
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
    public async Task ExecuteAsync_AsAdmin_UpdatesSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var owner = await SeedUser(UserType.PROFESSOR, 2);
        var cls = await SeedClass();
        await SeedRelation(cls.ClassId, owner.UserId, true);

        var updateDto = new ClassUpdateDTO
        {
            Id = cls.ClassId,
            ClassName = "New Name by Admin",
            Executor = new() { Id = admin.UserId, Role = UserType.ADMIN },
            Active = cls.Active!.Value,
            Color = cls.Color!,
            Section = cls.Section!,
            Subject = cls.Subject!,
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsOk);
        var updatedClass = await _ctx.Classes.FindAsync(cls.ClassId);
        Assert.NotNull(updatedClass);
        Assert.Equal("New Name by Admin", updatedClass.ClassName);
    }

    [Fact]
    public async Task ExecuteAsync_AsOwner_UpdatesSuccessfully()
    {
        var owner = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass();
        await SeedRelation(cls.ClassId, owner.UserId, true);

        var updateDto = new ClassUpdateDTO
        {
            Id = cls.ClassId,
            ClassName = "New Name by Owner",
            Executor = new() { Id = owner.UserId, Role = UserType.PROFESSOR },
            Active = cls.Active!.Value,
            Color = cls.Color!,
            Section = cls.Section!,
            Subject = cls.Subject!,
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsOk);
        var updatedClass = await _ctx.Classes.FindAsync(cls.ClassId);
        Assert.NotNull(updatedClass);
        Assert.Equal("New Name by Owner", updatedClass.ClassName);
    }

    [Fact]
    public async Task ExecuteAsync_AsNonOwnerProfessor_ReturnsUnauthorized()
    {
        var owner = await SeedUser(UserType.PROFESSOR, 1);
        var nonOwner = await SeedUser(UserType.PROFESSOR, 2);
        var cls = await SeedClass();
        await SeedRelation(cls.ClassId, owner.UserId, true);
        await SeedRelation(cls.ClassId, nonOwner.UserId, false); // Non-owner relation

        var updateDto = new ClassUpdateDTO
        {
            Id = cls.ClassId,
            ClassName = "New Name",
            Executor = new() { Id = nonOwner.UserId, Role = UserType.PROFESSOR },
            Active = cls.Active!.Value,
            Color = cls.Color!,
            Section = cls.Section!,
            Subject = cls.Subject!,
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AsStudent_ReturnsUnauthorized()
    {
        var student = await SeedUser(UserType.STUDENT);
        var cls = await SeedClass();

        var updateDto = new ClassUpdateDTO
        {
            Id = cls.ClassId,
            ClassName = "New Name",
            Executor = new Executor { Id = student.UserId, Role = UserType.STUDENT },
            Active = cls.Active!.Value,
            Color = cls.Color!,
            Section = cls.Section!,
            Subject = cls.Subject!,
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClass_ReturnsNotFound()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var updateDto = new ClassUpdateDTO
        {
            Id = "NON-EXISTENT",
            ClassName = "New Name",
            Executor = new() { Id = admin.UserId, Role = UserType.ADMIN },
            Active = true,
            Color = "#000000",
            Section = "Valid Section",
            Subject = "Valid Subject",
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidData_ReturnsInputError()
    {
        var owner = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass();
        await SeedRelation(cls.ClassId, owner.UserId, true);

        var updateDto = new ClassUpdateDTO
        {
            Id = cls.ClassId,
            ClassName = "", // Invalid name
            Executor = new Executor { Id = owner.UserId, Role = UserType.PROFESSOR },
            Active = cls.Active!.Value,
            Color = cls.Color!,
            Section = cls.Section!,
            Subject = cls.Subject!,
        };

        var result = await _useCase.ExecuteAsync(updateDto);

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
