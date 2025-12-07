using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.Services;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

public abstract class UpdateUseCase<I, UE, E>(
    IUpdaterAsync<E, UE> updater,
    IReaderAsync<I, E> reader,
    IBusinessValidationService<UE>? validator = null
) : IUseCaseAsync<UE, E>
    where I : notnull
    where UE : notnull
    where E : notnull
{
    /// <summary>
    /// Entidad encargada de actualizar un registro
    /// </summary>
    protected readonly IUpdaterAsync<E, UE> _updater = updater;

    /// <summary>
    /// Entidad encargada de leer un registro
    /// </summary>
    protected readonly IReaderAsync<I, E> _reader = reader;

    /// <summary>
    /// Entidad encargada de validar el formato de los campos de una entidad
    /// </summary>
    protected readonly IBusinessValidationService<UE>? _validator = validator;

    public async Task<Result<E, UseCaseError>> ExecuteAsync(UserActionDTO<UE> request)
    {
        var formatted = PreValidationFormat(request);

        if (_validator is not null)
        {
            var validation = _validator.IsValid(formatted.Data);
            if (validation.IsErr)
                return UseCaseErrors.Input(validation.UnwrapErr());
        }

        var record = await _reader.GetAsync(GetId(formatted.Data));
        if (record is null)
            return UseCaseErrors.NotFound();

        var syncCheck = ExtraValidation(formatted, record);
        if (syncCheck.IsErr)
            return syncCheck.UnwrapErr();

        var asyncCheck = await ExtraValidationAsync(formatted, record);
        if (asyncCheck.IsErr)
            return Result<E, UseCaseError>.Err(asyncCheck.UnwrapErr());

        var formattedSync = PostValidationFormat(formatted, record);
        var formattedAsync = await PostValidationFormatAsync(formattedSync, record);

        var updatedRecord = await _updater.UpdateAsync(formattedAsync.Data);

        ExtraTask(request, record, updatedRecord);
        await ExtraTaskAsync(request, record, updatedRecord);

        return updatedRecord;
    }

    protected virtual UserActionDTO<UE> PreValidationFormat(UserActionDTO<UE> value) => value;

    protected virtual Task<UserActionDTO<UE>> PostValidationFormatAsync(
        UserActionDTO<UE> value,
        E original
    ) => Task.FromResult(value);

    protected virtual UserActionDTO<UE> PostValidationFormat(UserActionDTO<UE> value, E original) =>
        value;

    protected virtual Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<UE> value,
        E original
    ) => Result<Unit, UseCaseError>.Ok(Unit.Value);

    protected virtual async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<UE> value,
        E original
    ) => Unit.Value;

    protected virtual void ExtraTask(UserActionDTO<UE> newEntity, E original, E createdEntity) { }

    protected virtual Task ExtraTaskAsync(
        UserActionDTO<UE> newEntity,
        E original,
        E createdEntity
    ) => Task.FromResult(Unit.Value);

    protected abstract I GetId(UE dto);
}
