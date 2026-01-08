using Application.DTOs.Common;
using Application.DTOs.Tests;
using Application.UseCases.Tests;
using Domain.Entities;
using Domain.Entities.Questions;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Application.DAOs;
using Application.Services.Validators;

namespace Tests.Application.UseCases.Tests;

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

public class UpdateTestUseCaseTest : BaseTest
{
    private readonly UpdateTestUseCase _useCase;

    public UpdateTestUseCaseTest()
    {
        var testUpdater = _sp.GetRequiredService<IUpdaterAsync<TestDomain, TestUpdateDTO>>();
        var testReader = _sp.GetRequiredService<IReaderAsync<Guid, TestDomain>>();
        var testValidator = new MockTestUpdateValidator();

        _useCase = new UpdateTestUseCase(testUpdater, testReader, testValidator);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndAdminRole_ReturnsOk()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser(UserType.PROFESSOR);

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
        var professor = await SeedUser(UserType.PROFESSOR);
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
        var professor = await SeedUser(UserType.PROFESSOR);
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
        var professor1 = await SeedUser(UserType.PROFESSOR);
        var professor2 = await SeedUser(UserType.PROFESSOR);
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
        var professor = await SeedUser(UserType.PROFESSOR);
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
}

