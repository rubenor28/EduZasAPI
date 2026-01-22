using Application.DTOs.ClassTests;
using Application.UseCases.ClassTests;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.UseCases.ClassTests;

public class AddClassTestUseCaseTest : BaseTest
{
    private readonly AddClassTestUseCase _useCase;

    public AddClassTestUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<AddClassTestUseCase>();
    }

    [Fact]
    public async Task ExecuteAsync_AsAdmin_WhenTestOwnerIsInClass_AddsSuccessfully()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(ownerId: professor.Id);
        var test = await SeedTest(professor.Id);

        var dto = new ClassTestIdDTO { ClassId = cls.Id, TestId = test.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_AsAdmin_WhenTestOwnerIsNotInClass_ReturnsUnauthorized()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(ownerId: admin.Id);

        var test = await SeedTest(professorId: professor.Id);
        // Note: Professor is NOT seeded into the class

        var dto = new ClassTestIdDTO { ClassId = cls.Id, TestId = test.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<UnauthorizedError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_AsProfessorAddingOwnTestAndInClass_AddsSuccessfully()
    {
        var professor = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(ownerId: professor.Id);
        var test = await SeedTest(professor.Id);

        var dto = new ClassTestIdDTO { ClassId = cls.Id, TestId = test.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(professor) }
        );

        Assert.True(result.IsOk);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClass_ReturnsInputError()
    {
        var professor = await SeedUser(UserType.PROFESSOR);
        var test = await SeedTest(professor.Id);
        var dto = new ClassTestIdDTO { ClassId = "non-existent", TestId = test.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(professor) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<InputError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenRelationAlreadyExists_ReturnsAlreadyExistsError()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var professor = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(professor.Id);
        var test = await SeedTest(professor.Id);

        var dto = new ClassTestIdDTO { ClassId = cls.Id, TestId = test.Id };
        await _useCase.ExecuteAsync(new() { Data = dto, Executor = AsExecutor(admin) }); // First time

        var result = await _useCase.ExecuteAsync(
            new() { Data = dto, Executor = AsExecutor(admin) }
        ); // Second time

        Assert.True(result.IsErr);
        Assert.IsType<Conflict>(result.UnwrapErr());
        var err = (Conflict)result.UnwrapErr();
        Assert.Equal("El recurso ya existe", err.Message);
    }
}
