using Application.DTOs.ClassTests;
using Application.UseCases.ClassTests;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.UseCases.ClassTests;

public class DeleteClassTestUseCaseTest : BaseTest
{
    private readonly DeleteClassTestUseCase _useCase;

    public DeleteClassTestUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<DeleteClassTestUseCase>();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndAdminRole_ReturnsOk()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser(UserType.PROFESSOR);
        var test = await SeedTest(professor.Id);
        var @class = await SeedClass(professor.Id);
        var classTest = await SeedClassTest(@class.Id, test.Id);

        var deleteDto = new ClassTestIdDTO
        {
            ClassId = classTest.ClassId,
            TestId = classTest.TestId,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = deleteDto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        Assert.NotNull(result.Unwrap());
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDataAndProfessorRole_ReturnsOk()
    {
        var professor = await SeedUser(UserType.PROFESSOR);
        var test = await SeedTest(professor.Id);
        var @class = await SeedClass(professor.Id);
        var classTest = await SeedClassTest(@class.Id, test.Id);

        var deleteDto = new ClassTestIdDTO
        {
            ClassId = classTest.ClassId,
            TestId = classTest.TestId,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = deleteDto, Executor = AsExecutor(professor) }
        );

        Assert.True(result.IsOk);
        Assert.NotNull(result.Unwrap());
    }

    [Fact]
    public async Task ExecuteAsync_RelationNotFound_ReturnsError()
    {
        var admin = await SeedUser(UserType.ADMIN);

        var deleteDto = new ClassTestIdDTO { ClassId = "non-existent", TestId = Guid.NewGuid() };

        var result = await _useCase.ExecuteAsync(
            new() { Data = deleteDto, Executor = AsExecutor(admin) }
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
        var @class = await SeedClass(professor.Id);
        var classTest = await SeedClassTest(@class.Id, test.Id);

        var deleteDto = new ClassTestIdDTO
        {
            ClassId = classTest.ClassId,
            TestId = classTest.TestId,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = deleteDto, Executor = AsExecutor(student) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_ProfessorDeletingAnotherProfessorsRelation_ReturnsUnauthorizedError()
    {
        var professor1 = await SeedUser(UserType.PROFESSOR);
        var professor2 = await SeedUser(UserType.PROFESSOR);
        var test = await SeedTest(professor1.Id);
        var @class = await SeedClass(professor1.Id);
        var classTest = await SeedClassTest(@class.Id, test.Id);

        var deleteDto = new ClassTestIdDTO
        {
            ClassId = classTest.ClassId,
            TestId = classTest.TestId,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = deleteDto, Executor = AsExecutor(professor2) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }
}
