using Application.DTOs.Common;
using Application.UseCases.Classes;
using Domain.Enums;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Classes;

public class DeleteClassUseCaseTest : IDisposable
{
    private readonly DeleteClassUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public DeleteClassUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var classMapper = new ClassMapper();
        var classProfessorMapper = new ClassProfessorMapper();

        var classDeleter = new ClassEFDeleter(_ctx, classMapper);
        var classReader = new ClassEFReader(_ctx, classMapper);
        var professorRelationReader = new ClassProfessorsEFReader(_ctx, classProfessorMapper);

        _useCase = new DeleteClassUseCase(classDeleter, classReader, professorRelationReader);
    }

    private async Task<User> SeedUser(UserType role, ulong userId = 1)
    {
        var user = new User
        {
            UserId = userId,
            Email = $"test{userId}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)role,
        };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return user;
    }

    private async Task<Class> SeedClass(ulong ownerId)
    {
        var cls = new Class { ClassId = "TEST-CLASS", ClassName = "Test Class" };
        _ctx.Classes.Add(cls);
        _ctx.ClassProfessors.Add(
            new ClassProfessor
            {
                ClassId = cls.ClassId,
                ProfessorId = ownerId,
                IsOwner = true,
            }
        );
        await _ctx.SaveChangesAsync();
        return cls;
    }

    [Fact]
    public async Task ExecuteAsync_AsAdmin_DeletesSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var owner = await SeedUser(UserType.PROFESSOR, 999); // Seed the owner
        var cls = await SeedClass(owner.UserId); // Use the real owner's ID

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = cls.ClassId,
                Executor = new() { Id = admin.UserId, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsOk);
        var deletedClass = await _ctx.Classes.FindAsync(cls.ClassId);
        Assert.Null(deletedClass); // Or check for soft delete flag if implemented
    }

    [Fact]
    public async Task ExecuteAsync_AsOwner_DeletesSuccessfully()
    {
        var owner = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(owner.UserId);

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = cls.ClassId,
                Executor = new() { Id = owner.UserId, Role = UserType.PROFESSOR },
            }
        );

        Assert.True(result.IsOk);
        var deletedClass = await _ctx.Classes.FindAsync(cls.ClassId);
        Assert.Null(deletedClass);
    }

    [Fact]
    public async Task ExecuteAsync_AsNonOwnerProfessor_ReturnsUnauthorized()
    {
        var owner = await SeedUser(UserType.PROFESSOR, 1);
        var nonOwner = await SeedUser(UserType.PROFESSOR, 2);
        var cls = await SeedClass(owner.UserId);

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = cls.ClassId,
                Executor = new() { Id = nonOwner.UserId, Role = UserType.PROFESSOR },
            }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AsStudent_ReturnsUnauthorized()
    {
        var student = await SeedUser(UserType.STUDENT);
        var owner = await SeedUser(UserType.PROFESSOR, 999); // Seed the owner
        var cls = await SeedClass(owner.UserId); // Use the real owner's ID

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = cls.ClassId,
                Executor = new() { Id = student.UserId, Role = UserType.STUDENT },
            }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClass_ReturnsNotFound()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = "NonExistent",
                Executor = new() { Id = admin.UserId, Role = UserType.ADMIN },
            }
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
