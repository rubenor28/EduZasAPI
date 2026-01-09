using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Answers;
using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Answers;

using AnswerCreator = ICreatorAsync<AnswerDomain, AnswerIdDTO>;
using ClassReader = IReaderAsync<string, ClassDomain>;
using ClassTestReader = IReaderAsync<ClassTestIdDTO, ClassTestDomain>;
using TestReader = IReaderAsync<Guid, TestDomain>;
using UserReader = IReaderAsync<ulong, UserDomain>;
using StudentReader = IReaderAsync<UserClassRelationId, ClassStudentDomain>;

public sealed class AddAnswerUseCase(
    AnswerCreator creator,
    TestReader testReader,
    ClassReader classReader,
    ClassTestReader classTestReader,
    UserReader userReader,
    StudentReader studentReader
) : AddUseCase<AnswerIdDTO, AnswerDomain>(creator, null)
{
    private readonly TestReader _testReader = testReader;
    private readonly ClassReader _classReader = classReader;
    private readonly UserReader _userReader = userReader;
    private readonly ClassTestReader _classTestReader = classTestReader;

    private readonly StudentReader _studentReader = studentReader;

    private async Task ItemExists<I, E>(
        I id,
        string fieldName,
        List<FieldErrorDTO> errors,
        Func<I, Task<E?>> searchFn
    )
    {
        var search = await searchFn(id);

        if (search is null)
            errors.Add(new() { Field = fieldName, Message = "No encontrado" });
    }

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<AnswerIdDTO> value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => value.Executor.Id == value.Data.UserId,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var errors = new List<FieldErrorDTO>();
        await ItemExists(value.Data.TestId, "testId", errors, _testReader.GetAsync);
        await ItemExists(value.Data.ClassId, "classId", errors, _classReader.GetAsync);
        await ItemExists(value.Data.UserId, "userId", errors, _userReader.GetAsync);

        if (errors.Any())
            return UseCaseErrors.Input(errors);

        var classTest = await _classTestReader.GetAsync(
            new() { TestId = value.Data.TestId, ClassId = value.Data.ClassId }
        );

        if (classTest is null)
            return UseCaseErrors.Unauthorized();

        var student = await _studentReader.GetAsync(new()
        {
            ClassId = value.Data.ClassId,
            UserId = value.Data.UserId,
        });

        if(student is null) return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
