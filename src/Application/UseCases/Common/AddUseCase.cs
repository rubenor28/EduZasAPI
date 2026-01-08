using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.Services.Validators;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Caso de uso base para la creación de nuevas entidades.
/// </summary>
/// <typeparam name="NE">Tipo del DTO de entrada para la creación.</typeparam>
/// <typeparam name="E">Tipo de la entidad de dominio a crear.</typeparam>
public abstract class AddUseCase<NE, E>(
    ICreatorAsync<E, NE> creator,
    IBusinessValidationService<NE>? validator = null
) : IUseCaseAsync<NE, E>
    where NE : notnull
    where E : notnull
{
    /// <summary>
    /// Entidad encargada de persistir una nueva entidad
    /// </summary>
    protected readonly ICreatorAsync<E, NE> _creator = creator;

    /// <summary>
    /// Entidad encargada de validar el formato de la entidad a insertar
    /// </summary>
    protected readonly IBusinessValidationService<NE>? _validator = validator;

    /// <inheritdoc/>
    public async virtual Task<Result<E, UseCaseError>> ExecuteAsync(UserActionDTO<NE> request)
    {
        var formattedRequest = PreValidationFormat(request);

        if (_validator is not null)
        {
            var validation = _validator.IsValid(formattedRequest.Data);
            if (validation.IsErr)
                return UseCaseErrors.Input(validation.UnwrapErr());
        }

        var syncCheck = ExtraValidation(formattedRequest);
        if (syncCheck.IsErr)
            return syncCheck.UnwrapErr();

        var asyncCheck = await ExtraValidationAsync(formattedRequest);
        if (asyncCheck.IsErr)
            return asyncCheck.UnwrapErr();

        var finalRequest = PostValidationFormat(formattedRequest);
        finalRequest = await PostValidationFormatAsync(finalRequest);

        PrevTask(finalRequest);
        await PrevTaskAsync(finalRequest);

        var newRecord = await _creator.AddAsync(finalRequest.Data);

        ExtraTask(finalRequest, newRecord);
        await ExtraTaskAsync(finalRequest, newRecord);

        return newRecord;
    }

    /// <summary>
    /// Formatea los datos antes de la validación.
    /// </summary>
    /// <param name="value">DTO de entrada original.</param>
    /// <returns>DTO formateado.</returns>
    protected virtual UserActionDTO<NE> PreValidationFormat(UserActionDTO<NE> value) => value;

    /// <summary>
    /// Formatea los datos después de la validación (asíncrono).
    /// </summary>
    /// <param name="value">DTO validado.</param>
    /// <returns>Tarea con el DTO formateado.</returns>
    protected virtual Task<UserActionDTO<NE>> PostValidationFormatAsync(UserActionDTO<NE> value) =>
        Task.FromResult(value);

    /// <summary>
    /// Formatea los datos después de la validación (síncrono).
    /// </summary>
    /// <param name="value">DTO validado.</param>
    /// <returns>DTO formateado.</returns>
    protected virtual UserActionDTO<NE> PostValidationFormat(UserActionDTO<NE> value) => value;

    /// <summary>
    /// Validaciones adicionales síncronas.
    /// </summary>
    /// <param name="value">DTO de entrada.</param>
    /// <returns>Resultado exitoso o error.</returns>
    protected virtual Result<Unit, UseCaseError> ExtraValidation(UserActionDTO<NE> value) =>
        Result<Unit, UseCaseError>.Ok(Unit.Value);

    /// <summary>
    /// Validaciones adicionales asíncronas.
    /// </summary>
    /// <param name="value">DTO de entrada.</param>
    /// <returns>Tarea con resultado exitoso o error.</returns>
    protected virtual Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NE> value
    ) => Task.FromResult(Result<Unit, UseCaseError>.Ok(Unit.Value));

    /// <summary>
    /// Tarea adicional a ejecutar después de la creación (síncrono).
    /// </summary>
    /// <param name="newEntity">DTO de entrada.</param>
    /// <param name="createdEntity">Entidad creada.</param>
    protected virtual void ExtraTask(UserActionDTO<NE> newEntity, E createdEntity) { }

    /// <summary>
    /// Tarea adicional a ejecutar después de la creación (asíncrono).
    /// </summary>
    /// <param name="newEntity">DTO de entrada.</param>
    /// <param name="createdEntity">Entidad creada.</param>
    protected virtual Task ExtraTaskAsync(UserActionDTO<NE> newEntity, E createdEntity) =>
        Task.CompletedTask;

    /// <summary>
    /// Tarea previa a la creación (síncrono).
    /// </summary>
    /// <param name="newEntity">DTO de entrada.</param>
    protected virtual void PrevTask(UserActionDTO<NE> newEntity) { }

    /// <summary>
    /// Tarea previa a la creación (asíncrono).
    /// </summary>
    /// <param name="newEntity">DTO de entrada.</param>
    protected virtual Task PrevTaskAsync(UserActionDTO<NE> newEntity) => Task.CompletedTask;
}
