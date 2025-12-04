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
        PreValidationFormat(ref request);

        if (_validator is not null)
        {
            var validation = _validator.IsValid(request.Data);
            if (validation.IsErr)
                return UseCaseErrors.Input(validation.UnwrapErr());
        }

        var record = await _reader.GetAsync(GetId(request.Data));
        if (record is null)
            return UseCaseErrors.NotFound();

        var syncCheck = ExtraValidation(request, record);
        if (syncCheck.IsErr)
            return syncCheck.UnwrapErr();

        var asyncCheck = await ExtraValidationAsync(request, record);
        if (asyncCheck.IsErr)
            return Result<E, UseCaseError>.Err(asyncCheck.UnwrapErr());

        PostValidationFormat(request, record);
        await PostValidationFormatAsync(request, record);

        var updatedRecord = await _updater.UpdateAsync(request.Data);

        ExtraTask(request, record, updatedRecord);
        await ExtraTaskAsync(request, record, updatedRecord);

        return updatedRecord;
    }

    protected virtual void PreValidationFormat(ref UserActionDTO<UE> value) { }

    protected virtual Task PostValidationFormatAsync(UserActionDTO<UE> value, E original) =>
        Task.CompletedTask;

    protected virtual void PostValidationFormat(UserActionDTO<UE> value, E original) { }

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
