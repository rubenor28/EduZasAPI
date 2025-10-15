using EduZasAPI.Application.Classes;
using EduZasAPI.Application.Common;
using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Infraestructure.EntityFramework.Application.Classes;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;
using EduZasAPI.Infraestructure.FluentValidation.Application.Classes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Application.Tests;

public class MockRandomStringGenerator : IRandomStringGeneratorService
{
    private int _counter = 0;
    public string Generate() => $"TESTID-{_counter++}";
}

public class AddClassUseCaseTest : IDisposable
{
    private readonly AddClassUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public AddClassUseCaseTest()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>()
          .UseSqlite(_conn)
          .Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var classRepository = new ClassEntityFrameworkRepository(_ctx, 10);
        var userRepository = new UserEntityFrameworkRepository(_ctx, 10);
        var professorRelationRepository = new ProfessorPerClassEntityFrameworkRepository(_ctx, 10);
        var validator = new NewClassFluentValidator();
        var idGenerator = new RandomStringGeneratorService("1234567890aabcdefghijklmnopqrstuvwxyz", 20);

        _useCase = new AddClassUseCase(classRepository, validator, userRepository, classRepository, idGenerator, professorRelationRepository);
    }

    private async Task SeedUser(UserType role)
    {
        var user = new User { UserId = 1, Email = "test@test.com", FirstName = "test", FatherLastname = "test", Password = "test", Role = (uint)role };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ReturnsOk()
    {
        await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("ABC"),
            Subject = Optional.Some("Math"),
            OwnerId = 1
        };

        var result = await _useCase.ExecuteAsync(newClass);

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ReturnsError()
    {
        var newClass = new NewClassDTO
        {
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("ABC"),
            Subject = Optional.Some("Math"),
            OwnerId = 1000
        };

        var result = await _useCase.ExecuteAsync(newClass);

        Assert.True(result.IsErr);

        var err = result.UnwrapErr();
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "ownerId");

    }

    [Fact]
    public async Task ExecuteAsync_UserNotAuthorized_ReturnsError()
    {
        await SeedUser(UserType.STUDENT);
        var newClass = new NewClassDTO
        {
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("ABC"),
            Subject = Optional.Some("Math"),
            OwnerId = 1
        };

        var result = await _useCase.ExecuteAsync(newClass);

        Assert.True(result.IsErr);
        Assert.Equal(typeof(UnauthorizedError), result.UnwrapErr().GetType());
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidClassName_ReturnsError()
    {
        await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            ClassName = "",
            Color = "#ffffff",
            Section = Optional.Some("ABC"),
            Subject = Optional.Some("Math"),
            OwnerId = 1
        };

        var result = await _useCase.ExecuteAsync(newClass);

        Assert.True(result.IsErr);

        var err = result.UnwrapErr();
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "className");
    }


    [Fact]
    public async Task ExecuteAsync_WithInvalidSection_ReturnsError()
    {
        await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            ClassName = "", // Invalid class name
            Color = "#ffffff",
            Section = Optional.Some("A"),
            Subject = Optional.Some("Math"),
            OwnerId = 1
        };

        var result = await _useCase.ExecuteAsync(newClass);

        Assert.True(result.IsErr);

        var err = result.UnwrapErr();
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "section");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidSubject_ReturnsError()
    {
        await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            ClassName = "", // Invalid class name
            Color = "#ffffff",
            Section = Optional.Some("ABC"),
            Subject = Optional.Some("T"),
            OwnerId = 1
        };

        var result = await _useCase.ExecuteAsync(newClass);

        Assert.True(result.IsErr);

        var err = result.UnwrapErr();
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "subject");
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
