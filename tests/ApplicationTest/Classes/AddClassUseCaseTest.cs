using Application.DTOs.Classes;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Classes;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using FluentValidationProj.Application.Services.Classes;
using InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Classes;

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

    private readonly UserEFMapper _userMapper;

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

        var userTypeMapper = new UserTypeMapper();
        _userMapper = new UserEFMapper(userTypeMapper, userTypeMapper);

        var classMapper = new ClassEFMapper();
        var professorClassMapper = new ProfessorClassEFMapper();
        var classCreator = new ClassEFCreator(_ctx, classMapper, classMapper);

        var professorClassCreator = new ClassProfessorEFCreator(
            _ctx,
            professorClassMapper,
            professorClassMapper
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
        var user = new User
        {
            UserId = 1,
            Email = "test@test.com",
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
            OwnerId = 1,
            Executor = AsExecutor(user),
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
            Section = "ABC",
            Subject = "Math",
            OwnerId = 1000,
            Executor = new() { Id = 1, Role = UserType.ADMIN },
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
        var user = await SeedUser(UserType.STUDENT);
        var newClass = new NewClassDTO
        {
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = "ABC",
            Subject = "Math",
            OwnerId = 1,
            Executor = AsExecutor(user)
        };

        var result = await _useCase.ExecuteAsync(newClass);

        Assert.True(result.IsErr);
        Assert.Equal(typeof(UnauthorizedError), result.UnwrapErr().GetType());
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
            OwnerId = 1,
            Executor = AsExecutor(user)
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
        var user = await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            ClassName = "", // Invalid class name
            Color = "#ffffff",
            Section = "A",
            Subject = "Math",
            OwnerId = 1,
            Executor = AsExecutor(user)
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
        var user = await SeedUser(UserType.PROFESSOR);
        var newClass = new NewClassDTO
        {
            ClassName = "", // Invalid class name
            Color = "#ffffff",
            Section = "ABC",
            Subject = "T",
            OwnerId = 1,
            Executor = AsExecutor(user)
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
