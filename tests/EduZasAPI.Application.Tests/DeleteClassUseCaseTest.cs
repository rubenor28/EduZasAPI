using EduZasAPI.Application.Classes;
using EduZasAPI.Application.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Infraestructure.EntityFramework.Application.Classes;
using EduZasAPI.Infraestructure.EntityFramework.Application.ClassProfessors;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Application.Tests;

public class DeleteClassUseCaseTest : IDisposable
{
    private readonly DeleteClassUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public DeleteClassUseCaseTest()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var classRepository = new ClassEntityFrameworkRepository(_ctx, 10);
        var professorRelationRepository = new ProfessorPerClassEntityFrameworkRepository(_ctx, 10);

        _useCase = new DeleteClassUseCase(
            classRepository,
            classRepository,
            professorRelationRepository
        );
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

        var deleteDto = new DeleteClassDTO
        {
            Id = cls.ClassId,
            Executor = new Executor { Id = admin.UserId, Role = UserType.ADMIN }
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsOk);
        var deletedClass = await _ctx.Classes.FindAsync(cls.ClassId);
        Assert.Null(deletedClass); // Or check for soft delete flag if implemented
    }

    [Fact]
    public async Task ExecuteAsync_AsOwner_DeletesSuccessfully()
    {
        var owner = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(owner.UserId);

        var deleteDto = new DeleteClassDTO
        {
            Id = cls.ClassId,
            Executor = new Executor { Id = owner.UserId, Role = UserType.PROFESSOR },
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

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

        var deleteDto = new DeleteClassDTO
        {
            Id = cls.ClassId,
            Executor = new Executor { Id = nonOwner.UserId, Role = UserType.PROFESSOR },
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AsStudent_ReturnsUnauthorized()
    {
        var student = await SeedUser(UserType.STUDENT);
        var owner = await SeedUser(UserType.PROFESSOR, 999); // Seed the owner
        var cls = await SeedClass(owner.UserId); // Use the real owner's ID

        var deleteDto = new DeleteClassDTO
        {
            Id = cls.ClassId,
            Executor = new Executor { Id = student.UserId, Role = UserType.STUDENT }
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClass_ReturnsNotFound()
    {
        var admin = await SeedUser(UserType.ADMIN);

        var deleteDto = new DeleteClassDTO
        {
            Id = "NON-EXISTENT-ID",
            Executor = new Executor { Id = admin.UserId, Role = UserType.ADMIN },
        };

        var result = await _useCase.ExecuteAsync(deleteDto);

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
