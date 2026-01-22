using Application.DAOs;
using Application.DTOs.Answers;
using Application.DTOs.ClassTests;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassTests;

/// <summary>
/// Caso de uso para eliminar la asociación de una evaluación con una clase.
/// </summary>
public sealed class DeleteClassTestUseCase(
    IDeleterAsync<ClassTestIdDTO, ClassTestDomain> deleter,
    IReaderAsync<ClassTestIdDTO, ClassTestDomain> reader,
    IReaderAsync<Guid, TestDomain> testReader,
    IQuerierAsync<AnswerDomain, AnswerCriteriaDTO> answerQuerier,
    IDeleterAsync<AnswerIdDTO, AnswerDomain> answerDeleter,
    IBusinessValidationService<ClassTestIdDTO>? validator = null
) : DeleteUseCase<ClassTestIdDTO, ClassTestDomain>(deleter, reader, validator)
{
    private readonly IReaderAsync<Guid, TestDomain> _testReader = testReader;
    private readonly IQuerierAsync<AnswerDomain, AnswerCriteriaDTO> _answerQuerier = answerQuerier;
    private readonly IDeleterAsync<AnswerIdDTO, AnswerDomain> _answerDeleter = answerDeleter;

    private async Task<bool> IsAuthorizedProfessor(
        ClassTestDomain testClassRelation,
        Executor executor
    )
    {
        var test = await _testReader.GetAsync(testClassRelation.TestId);
        return test is null
            ? throw new InvalidDataException("No se encontró el test de la relación")
            : test.ProfessorId == executor.Id;
    }

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassTestIdDTO> value,
        ClassTestDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsAuthorizedProfessor(record, value.Executor),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <inheritdoc/>
    protected override async Task ExtraTaskAsync(
        UserActionDTO<ClassTestIdDTO> dto,
        ClassTestDomain deleted
    )
    {
        var answers = (
            await _answerQuerier.GetByAsync(
                new() { ClassId = dto.Data.ClassId, TestId = dto.Data.TestId }
            )
        ).Results.Select(a => new AnswerIdDTO
        {
            ClassId = a.ClassId,
            TestId = a.TestId,
            UserId = a.UserId,
        });

        await _answerDeleter.BulkDelete(answers);
    }
}
