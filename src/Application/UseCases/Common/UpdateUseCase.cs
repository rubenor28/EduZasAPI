using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.Services.Validators;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Caso de uso base para la actualización de entidades existentes.
/// </summary>
/// <typeparam name="I">Tipo del identificador de la entidad.</typeparam>
/// <typeparam name="UE">Tipo del DTO de entrada con los datos a actualizar.</typeparam>
/// <typeparam name="E">Tipo de la entidad de dominio a actualizar.</typeparam>
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

    /// <inheritdoc/>
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

    /// <summary>
    /// Formatea los datos antes de la validación.
    /// </summary>
    /// <param name="value">DTO de entrada original.</param>
    /// <returns>DTO formateado.</returns>
    protected virtual UserActionDTO<UE> PreValidationFormat(UserActionDTO<UE> value) => value;

    /// <summary>
    /// Formatea los datos después de la validación (asíncrono).
    /// </summary>
    /// <param name="value">DTO validado.</param>
    /// <param name="original">Entidad original.</param>
    /// <returns>Tarea con el DTO formateado.</returns>
    protected virtual Task<UserActionDTO<UE>> PostValidationFormatAsync(
        UserActionDTO<UE> value,
        E original
    ) => Task.FromResult(value);

    /// <summary>
    /// Formatea los datos después de la validación (síncrono).
    /// </summary>
    /// <param name="value">DTO validado.</param>
    /// <param name="original">Entidad original.</param>
    /// <returns>DTO formateado.</returns>
    protected virtual UserActionDTO<UE> PostValidationFormat(UserActionDTO<UE> value, E original) =>
        value;

    /// <summary>
    /// Validaciones adicionales síncronas.
    /// </summary>
    /// <param name="value">DTO de entrada.</param>
    /// <param name="original">Entidad original.</param>
    /// <returns>Resultado exitoso o error.</returns>
    protected virtual Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<UE> value,
        E original
    ) => Result<Unit, UseCaseError>.Ok(Unit.Value);

    /// <summary>
    /// Validaciones adicionales asíncronas.
    /// </summary>
    /// <param name="value">DTO de entrada.</param>
    /// <param name="original">Entidad original.</param>
    /// <returns>Tarea con resultado exitoso o error.</returns>
    protected virtual async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<UE> value,
        E original
    ) => Unit.Value;

    /// <summary>
    /// Tarea adicional después de actualizar (síncrono).
    /// </summary>
    /// <param name="newEntity">DTO de entrada.</param>
    /// <param name="original">Entidad original.</param>
    /// <param name="createdEntity">Entidad actualizada.</param>
    protected virtual void ExtraTask(UserActionDTO<UE> newEntity, E original, E createdEntity) { }

    /// <summary>
    /// Tarea adicional después de actualizar (asíncrono).
    /// </summary>
    /// <param name="newEntity">DTO de entrada.</param>
    /// <param name="original">Entidad original.</param>
    /// <param name="createdEntity">Entidad actualizada.</param>
    protected virtual Task ExtraTaskAsync(
        UserActionDTO<UE> newEntity,
        E original,
        E createdEntity
    ) => Task.FromResult(Unit.Value);

    /// <summary>
    /// Obtiene el ID de la entidad desde el DTO.
    /// </summary>
    /// <param name="dto">DTO de actualización.</param>
    /// <returns>Identificador de la entidad.</returns>
    protected abstract I GetId(UE dto);
}
