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

    /// <summary>
    /// Valida asíncronamente que no existan respuestas para el examen antes de permitir su actualización y verifica la autorización.
    /// </summary>
    /// <param name="value">Datos de la actualización.</param>
    /// <param name="record">Entidad del examen original.</param>
    /// <returns>Resultado exitoso o error de caso de uso.</returns>
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

    /// <summary>
    /// Obtiene el identificador del examen desde el DTO de actualización.
    /// </summary>
    /// <param name="dto">DTO de actualización.</param>
    /// <returns>ID del examen.</returns>
    protected override Guid GetId(TestUpdateDTO dto) => dto.Id;
}
