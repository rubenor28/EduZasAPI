using Application.DTOs.Common;
using Application.DTOs.Tests;
using Application.Services;
using Application.UseCases.Tests;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Tests;

class MockTestUpdateValidator : IBusinessValidationService<TestUpdateDTO>
{
    public Result<Unit, IEnumerable<FieldErrorDTO>> IsValid(TestUpdateDTO data)
    {
        List<FieldErrorDTO> errors = [];

        if (string.IsNullOrEmpty(data.Content.Trim()))
            errors.Add(new() { Field = "content", Message = "Campo obligatorio" });

        if (string.IsNullOrEmpty(data.Title.Trim()))
            errors.Add(new() { Field = "title", Message = "Campo obligatorio" });

        if (errors.Count > 0)
            return errors;

        return Unit.Value;
    }
}

public class UpdateTestUseCaseTest : IDisposable
{
    private readonly UpdateTestUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly TestEFMapper _testMapper = new();
    private readonly UserEFMapper _userMapper;

    private readonly Random _random = new();

    public UpdateTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testUpdater = new TestEFUpdater(_ctx, _testMapper, _testMapper);
        var testReader = new TestEFReader(_ctx, _testMapper);
        var testValidator = new MockTestUpdateValidator();

        _useCase = new UpdateTestUseCase(testUpdater, testReader, testValidator);

        var userTypeMapper = new UserTypeMapper();
        _userMapper = new(userTypeMapper, userTypeMapper);
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.PROFESSOR)
    {
        var id = (ulong)_random.Next(1000, 100000);

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

    private async Task<TestDomain> SeedTest(ulong professorId)
    {
        var test = new Test
        {
            Title = "Original Title",
            Content = "Original Content",
            ProfessorId = professorId,
        };
        _ctx.Tests.Add(test);
        await _ctx.SaveChangesAsync();
        return _testMapper.Map(test);
    }

    private static Executor AsExecutor(UserDomain user) => new() { Id = user.Id, Role = user.Role };

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndAdminRole_ReturnsOk()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser();
        var test = await SeedTest(professor.Id);

        var updateDto = new TestUpdateDTO
        {
            Id = test.Id,
            Title = "Updated Title",
            Content = "Updated Content",
            ProfessorId = professor.Id,
            Executor = AsExecutor(admin),
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndProfessorRole_ReturnsOk()
    {
        var professor = await SeedUser();
        var test = await SeedTest(professor.Id);

        var updateDto = new TestUpdateDTO
        {
            Id = test.Id,
            Title = "Updated Title",
            Content = "Updated Content",
            ProfessorId = professor.Id,
            Executor = AsExecutor(professor),
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_TestNotFound_ReturnsError()
    {
        var admin = await SeedUser(UserType.ADMIN);

        var updateDto = new TestUpdateDTO
        {
            Id = 999, // Non-existent test
            Title = "Updated Title",
            Content = "Updated Content",
            ProfessorId = 1,
            Executor = AsExecutor(admin),
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithStudentRole_ReturnsUnauthorizedError()
    {
        var student = await SeedUser(UserType.STUDENT);
        var professor = await SeedUser();
        var test = await SeedTest(professor.Id);

        var updateDto = new TestUpdateDTO
        {
            Id = test.Id,
            Title = "Updated Title",
            Content = "Updated Content",
            ProfessorId = professor.Id,
            Executor = AsExecutor(student),
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_ProfessorUpdatingAnotherProfessorsTest_ReturnsUnauthorizedError()
    {
        var professor1 = await SeedUser();
        var professor2 = await SeedUser();
        var test = await SeedTest(professor1.Id);

        var updateDto = new TestUpdateDTO
        {
            Id = test.Id,
            Title = "Updated Title",
            Content = "Updated Content",
            ProfessorId = professor1.Id,
            Executor = AsExecutor(professor2),
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidTitle_ReturnsInputError()
    {
        var professor = await SeedUser();
        var test = await SeedTest(professor.Id);

        var updateDto = new TestUpdateDTO
        {
            Id = test.Id,
            Title = "", // Invalid title
            Content = "Updated Content",
            ProfessorId = 1,
            Executor = AsExecutor(professor),
        };

        var result = await _useCase.ExecuteAsync(updateDto);

        Assert.True(result.IsErr);
        var err = result.UnwrapErr();
        Assert.IsType<InputError>(err);
        Assert.Contains(((InputError)err).Errors, e => e.Field == "title");
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
