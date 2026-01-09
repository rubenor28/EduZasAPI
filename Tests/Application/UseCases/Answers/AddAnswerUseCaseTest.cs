using Application.DTOs.Answers;
using Application.DTOs.Common;
using Application.UseCases.Answers;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.UseCases.Answers;

public class AddAnswerUseCaseTest : BaseTest
{
    private readonly AddAnswerUseCase _useCase;

    public AddAnswerUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<AddAnswerUseCase>();
    }

    [Fact]
    public async Task AddAnswer_ReturnsCreated()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var student = await SeedUser(UserType.STUDENT);
        var cls = await SeedClass(ownerId: professor.Id);
        var test = await SeedTest(professorId: professor.Id);
        await SeedClassTest(classId: cls.Id, testId: test.Id);
        await SeedClassStudent(classId: cls.Id, studentId: student.Id);

        // Act
        var newAnswer = new AnswerIdDTO
        {
            ClassId = cls.Id,
            TestId = test.Id,
            UserId = student.Id,
        };

        var execution = await _useCase.ExecuteAsync(
            new() { Data = newAnswer, Executor = AsExecutor(student) }
        );

        // Arrange
        Assert.True(execution.IsOk);
        var answer = execution.Unwrap();
        Assert.Equal(newAnswer.ClassId, answer.ClassId);
        Assert.Equal(newAnswer.TestId, answer.TestId);
        Assert.Equal(newAnswer.UserId, answer.UserId);
    }

    [Theory]
    [InlineData(UserType.STUDENT, true)]
    [InlineData(UserType.STUDENT, false)]
    [InlineData(UserType.PROFESSOR, true)]
    [InlineData(UserType.PROFESSOR, false)]
    public async Task AddAnswer_WithNoAuthorizedUser_ReturnsUnauthorized(
        UserType unauthorizedUsrRole,
        bool enrollUnauthorized
    )
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var student = await SeedUser(UserType.STUDENT);
        var cls = await SeedClass(ownerId: professor.Id);
        var test = await SeedTest(professorId: professor.Id);
        await SeedClassTest(classId: cls.Id, testId: test.Id);
        await SeedClassStudent(classId: cls.Id, studentId: student.Id);

        var unauthorized = await SeedUser(unauthorizedUsrRole);
        if (enrollUnauthorized)
            await SeedClassStudent(classId: cls.Id, studentId: unauthorized.Id);

        // Act
        var newAnswer = new AnswerIdDTO
        {
            ClassId = cls.Id,
            TestId = test.Id,
            UserId = student.Id,
        };

        var execution = await _useCase.ExecuteAsync(
            new() { Data = newAnswer, Executor = AsExecutor(unauthorized) }
        );

        // Assert
        Assert.True(execution.IsErr);
        Assert.IsType<UnauthorizedError>(execution.UnwrapErr());
    }

    [Fact]
    public async Task AddAnswer_WithNonExistentDependencies_RetunsError()
    {
        // Arrange
        var newAnswer = new AnswerIdDTO
        {
            ClassId = "non-exists",
            UserId = 999,
            TestId = Guid.NewGuid(),
        };

        // Act
        var execution = await _useCase.ExecuteAsync(
            new()
            {
                Data = newAnswer,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

        // Assert
        Assert.True(execution.IsErr);
        var error = Assert.IsType<InputError>(execution.UnwrapErr());
        Assert.Contains(error.Errors, e => e.Field == "classId");
        Assert.Contains(error.Errors, e => e.Field == "userId");
        Assert.Contains(error.Errors, e => e.Field == "testId");
    }
}

