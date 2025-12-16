using Application.DTOs.Common;
using Application.DTOs.Tests;
using Application.Services;
using Application.UseCases.Tests;
using Domain.Entities;
using Domain.Entities.Questions;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Tests;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Tests;

class MockTestUpdateValidator : IBusinessValidationService<TestUpdateDTO>
{
    public Result<Unit, IEnumerable<FieldErrorDTO>> IsValid(TestUpdateDTO data)
    {
        List<FieldErrorDTO> errors = [];

        if (data.Content is null)
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

    private readonly TestMapper _testMapper = new();
    private readonly UserMapper _userMapper = new();

    private readonly Random _random = new();

    public UpdateTestUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var testUpdater = new TestEFUpdater(_ctx, _testMapper, new UpdateTestEFMapper());
        var testReader = new TestEFReader(_ctx, _testMapper);
        var testValidator = new MockTestUpdateValidator();

        _useCase = new UpdateTestUseCase(testUpdater, testReader, testValidator);
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

    private async Task<TestDomain> SeedTest(
        ulong professorId,
        IDictionary<Guid, IQuestion>? content = null
    )
    {
        var test = new Test
        {
            Title = "Original Title",
            Content = content ?? new Dictionary<Guid, IQuestion>(),
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

        var initialQuestionId = Guid.NewGuid();
        var initialContent = new Dictionary<Guid, IQuestion>
        {
            {
                initialQuestionId,
                new MultipleChoiseQuestion
                {
                    Title = "Initial Question",
                    ImageUrl = null,
                    Options = new Dictionary<Guid, string>(),
                    CorrectOption = Guid.Empty,
                }
            },
        };

        var test = await SeedTest(professor.Id, initialContent);

        var updatedQuestionId = Guid.NewGuid();
        var updatedContent = new Dictionary<Guid, IQuestion>
        {
            {
                updatedQuestionId,
                new OpenQuestion()
                {
                    Title = "Updated Open Question",
                    ImageUrl = "http://updated.image/path",
                }
            },
        };

        var updateDto = new TestUpdateDTO
        {
            Id = test.Id,
            Color = "#ffffff",
            Active = true,
            Title = "Updated Title",
            Content = updatedContent, 
            ProfessorId = professor.Id,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = updateDto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var testUpdated = result.Unwrap();

        Assert.Equal("Updated Title", testUpdated.Title);
        Assert.Equal("#ffffff", testUpdated.Color);
        Assert.True(testUpdated.Active);
        Assert.Equal(professor.Id, testUpdated.ProfessorId);

        Assert.Single(testUpdated.Content);
        Assert.True(testUpdated.Content.ContainsKey(updatedQuestionId));

        var updatedQuestion = testUpdated.Content[updatedQuestionId];
        Assert.IsType<OpenQuestion>(updatedQuestion);
        Assert.Equal("Updated Open Question", updatedQuestion.Title);
        Assert.Equal("http://updated.image/path", updatedQuestion.ImageUrl);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndProfessorRole_ReturnsOk()
    {
        var professor = await SeedUser();
        var test = await SeedTest(professor.Id);

        var updateDto = new TestUpdateDTO
        {
            Id = test.Id,
            Color = "#ffffff",
            Active = true,
            Title = "Updated Title",
            Content = test.Content,
            ProfessorId = professor.Id,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = updateDto, Executor = AsExecutor(professor) }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_TestNotFound_ReturnsError()
    {
        var admin = await SeedUser(UserType.ADMIN);

        var updateDto = new TestUpdateDTO
        {
            Id = Guid.NewGuid(), 
            Color = "#ffffff",
            Active = true,
            Title = "Updated Title",
            Content = new Dictionary<Guid, IQuestion>(),
            ProfessorId = 1,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = updateDto, Executor = AsExecutor(admin) }
        );

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
            Color = "#ffffff",
            Active = true,
            Title = "Updated Title",
            Content = test.Content,
            ProfessorId = professor.Id,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = updateDto, Executor = AsExecutor(student) }
        );

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
            Color = "#ffffff",
            Active = true,
            Title = "Updated Title",
            Content = test.Content,
            ProfessorId = professor1.Id,
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = updateDto,

                Executor = AsExecutor(professor2),
            }
        );

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
            Color = "#ffffff",
            Active = true,
            Title = "",
            Content = test.Content,
            ProfessorId = 1,
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = updateDto,

                Executor = AsExecutor(professor),
            }
        );

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
