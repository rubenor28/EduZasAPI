using Application.DTOs.Classes;
using Application.Services;
using Application.UseCases.Classes;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using FluentValidationProj.Application.Services.Classes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests.Application.UseCases.Classes;

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

    private readonly UserMapper _userMapper = new();
    private readonly Random _rdm = new();

    public AddClassUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var idGenerator = new RandomStringGeneratorService(
            "1234567890aabcdefghijklmnopqrstuvwxyz".ToCharArray(),
            20
        );

        var classMapper = new ClassMapper();
        var professorClassMapper = new ClassProfessorMapper();
        var classCreator = new ClassEFCreator(_ctx, classMapper, new NewClassEFMapper());

        var professorClassCreator = new ClassProfessorsEFCreator(
            _ctx,
            professorClassMapper,
            new NewClassProfessorEFMapper()
        );

        var classValidator = new NewClassFluentValidator();

        var userReader = new UserEFReader(_ctx, _userMapper);
        var classReader = new ClassEFReader(_ctx, classMapper);

        _useCase = new AddClassUseCase(
            classCreator,
            classValidator,
            userReader,
            classReader,
            idGenerator,
            professorClassCreator
        );
    }

    private async Task<UserDomain> SeedUser(UserType role)
    {
        var id = (ulong)_rdm.NextInt64();
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

    public static Executor AsExecutor(UserDomain user) => new() { Id = user.Id, Role = user.Role };

    [Fact]
    public async Task ExecuteAsync_WithValidData_ReturnsOk()
    {
        var user = await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = "ABC",
            Subject = "Math",
            OwnerId = user.Id,
            Professors = [],
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = newClass, Executor = AsExecutor(user) }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ReturnsError()
    {
        var newClass = new NewClassDTO
        {
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = "ABC",
            Subject = "Math",
            OwnerId = 1000,
            Professors = [],
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newClass,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsErr);

        var err = result.UnwrapErr();
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "ownerId");
    }

    [Fact]
    public async Task ExecuteAsync_UserNotAuthorized_ReturnsError()
    {
        var user = await SeedUser(UserType.STUDENT);
        var newClass = new NewClassDTO
        {
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = "ABC",
            Subject = "Math",
            OwnerId = user.Id,
            Professors = [],
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = newClass, Executor = AsExecutor(user) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidClassName_ReturnsError()
    {
        var user = await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            ClassName = "",
            Color = "#ffffff",
            Section = "ABC",
            Subject = "Math",
            OwnerId = user.Id,
            Professors = [],
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = newClass, Executor = AsExecutor(user) }
        );

        Assert.True(result.IsErr);

        var err = result.UnwrapErr();
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "className");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidSection_ReturnsError()
    {
        var user = await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            ClassName = "", // Invalid class name
            Color = "#ffffff",
            Section = "A",
            Subject = "Math",
            OwnerId = user.Id,
            Professors = [],
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = newClass, Executor = AsExecutor(user) }
        );

        Assert.True(result.IsErr);

        var err = result.UnwrapErr();
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "section");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidSubject_ReturnsError()
    {
        var user = await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            ClassName = "", // Invalid class name
            Color = "#ffffff",
            Section = "ABC",
            Subject = "T",
            OwnerId = user.Id,
            Professors = [],
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = newClass, Executor = AsExecutor(user) }
        );

        Assert.True(result.IsErr);

        var err = result.UnwrapErr();
        Assert.Equal(typeof(InputError), err.GetType());
        Assert.Contains(((InputError)err).Errors, e => e.Field == "subject");
    }

    [Fact]
    public async Task ExecuteAsync_WithProfessors_CreatesProfessors()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var prof2 = await SeedUser(UserType.PROFESSOR);
        var prof3 = await SeedUser(UserType.PROFESSOR);

        var newClass = new NewClassDTO
        {
            ClassName = "Multi-Professor Class",
            Color = "#123456",
            Section = "101",
            Subject = "Advanced Testing",
            OwnerId = owner.Id,
            Professors =
            [
                new() { UserId = prof2.Id, IsOwner = false },
                new() { UserId = prof3.Id, IsOwner = false },
            ],
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = newClass, Executor = AsExecutor(owner) }
        );

        // Assert
        if (result.IsErr)
        {
            Assert.Fail($"Test failed with an unexpected error: {result.UnwrapErr()}");
        }
        Assert.True(result.IsOk);

        var createdClass = result.Unwrap();
        var professorRelations = await _ctx
            .ClassProfessors.Where(cp => cp.ClassId == createdClass.Id)
            .ToListAsync();

        Assert.Equal(3, professorRelations.Count);
        Assert.Contains(professorRelations, pr => pr.ProfessorId == owner.Id && (bool)pr.IsOwner!);
        Assert.Contains(professorRelations, pr => pr.ProfessorId == prof2.Id && (bool)!pr.IsOwner!);
        Assert.Contains(professorRelations, pr => pr.ProfessorId == prof3.Id && (bool)!pr.IsOwner!);
    }

    [Fact]
    public async Task ExecuteAsync_WithStudentAsProfessor_ReturnsError()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var student = await SeedUser(UserType.STUDENT);

        var newClass = new NewClassDTO
        {
            ClassName = "Class with Invalid Professor",
            Color = "#123456",
            Section = "101",
            Subject = "Error Handling",
            OwnerId = owner.Id,
            Professors = [new() { UserId = student.Id, IsOwner = false }],
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = newClass, Executor = AsExecutor(owner) }
        );

        // Assert
        Assert.True(result.IsErr);

        var err = result.UnwrapErr();
        Assert.IsType<InputError>(err);
        Assert.Contains(((InputError)err).Errors, e => e.Field == "professors");
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
