using Application.DAOs;
using Application.DTOs.Answers;
using Application.DTOs.Tests;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Tests;

/// <summary>
/// Caso de uso para actualizar una evaluación.
/// </summary>
public sealed class UpdateTestUseCase(
    IUpdaterAsync<TestDomain, TestUpdateDTO> updater,
    IReaderAsync<Guid, TestDomain> reader,
    IBusinessValidationService<TestUpdateDTO> validator,
    IQuerierAsync<AnswerDomain, AnswerCriteriaDTO> answerQuerier
) : UpdateUseCase<Guid, TestUpdateDTO, TestDomain>(updater, reader, validator)
{
    readonly IQuerierAsync<AnswerDomain, AnswerCriteriaDTO> _answerQuerier = answerQuerier;

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<TestUpdateDTO> value,
        TestDomain record
    )
    {
        var hasAnswers = await _answerQuerier.AnyAsync(new() { TestId = value.Data.Id });

        if (hasAnswers)
            return UseCaseErrors.Conflict("No se puede modificar una evaluación con respuestas");

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Executor.Id == value.Data.ProfessorId,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <inheritdoc/>
    protected override Guid GetId(TestUpdateDTO dto) => dto.Id;
}
