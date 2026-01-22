using Application.DAOs;
using Application.Services.Validators;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Caso de uso genérico para eliminar una entidad del sistema.
/// </summary>
/// <typeparam name="I">El tipo del identificador de la entidad.</typeparam>
/// <typeparam name="I">El tipo del DTO de entrada para la eliminación.</typeparam>
/// <typeparam name="E">El tipo de la entidad de dominio que se eliminará.</typeparam>
public abstract class DeleteUseCase<I, E>(
    IDeleterAsync<I, E> deleter,
    IReaderAsync<I, E> reader,
    IBusinessValidationService<I>? validator = null
) : IUseCaseAsync<I, E>
    where I : notnull
    where E : notnull
{
    /// <summary>
    /// Entidad encargada de eliminar una entidad de un medio persistente
    /// </summary>
    protected readonly IDeleterAsync<I, E> _deleter = deleter;

    /// <summary>
    /// Entidad encargada de buscar una entidad de un medio persistente por ID
    /// </summary>
    protected readonly IReaderAsync<I, E> _reader = reader;

    /// <summary>
    /// Entidad encargada de validar formato de las propiedades de una entidad
    /// </summary>
    protected readonly IBusinessValidationService<I>? _validator = validator;

    /// <inheritdoc/>
    public async Task<Result<E, UseCaseError>> ExecuteAsync(UserActionDTO<I> request)
    {
        if (_validator is not null)
        {
            var validation = _validator.IsValid(request.Data);
            if (validation.IsErr)
                return UseCaseErrors.Input(validation.UnwrapErr());
        }

        var record = await _reader.GetAsync(request.Data);
        if (record is null)
            return UseCaseErrors.NotFound();

        var syncCheck = ExtraValidation(request, record);
        if (syncCheck.IsErr)
            return syncCheck.UnwrapErr();

        var asyncCheck = await ExtraValidationAsync(request, record);
        if (asyncCheck.IsErr)
            return asyncCheck.UnwrapErr();

        PrevTask(request, record);
        await PrevTaskAsync(request, record);

        await _deleter.DeleteAsync(request.Data);

        ExtraTask(request, record);
        await ExtraTaskAsync(request, record);
        return record;
    }

    /// <summary>
    /// Validaciones adicionales síncronas antes de eliminar.
    /// </summary>
    /// <param name="value">DTO de entrada.</param>
    /// <param name="record">Entidad a eliminar.</param>
    /// <returns>Resultado exitoso o error.</returns>
    protected virtual Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<I> value,
        E record
    ) => Result<Unit, UseCaseError>.Ok(Unit.Value);

    /// <summary>
    /// Validaciones adicionales asíncronas antes de eliminar.
    /// </summary>
    /// <param name="value">DTO de entrada.</param>
    /// <param name="record">Entidad a eliminar.</param>
    /// <returns>Tarea con resultado exitoso o error.</returns>
    protected virtual async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<I> value,
        E record
    ) => Unit.Value;

    /// <summary>
    /// Tarea adicional después de eliminar (síncrono).
    /// </summary>
    /// <param name="deleteDTO">DTO de entrada.</param>
    /// <param name="deletedEntity">Entidad eliminada.</param>
    protected virtual void ExtraTask(UserActionDTO<I> deleteDTO, E deletedEntity) { }

    /// <summary>
    /// Tarea adicional después de eliminar (asíncrono).
    /// </summary>
    /// <param name="deleteDTO">DTO de entrada.</param>
    /// <param name="deletedEntity">Entidad eliminada.</param>
    protected virtual Task ExtraTaskAsync(UserActionDTO<I> deleteDTO, E deletedEntity) =>
        Task.FromResult(Unit.Value);

    /// <summary>
    /// Tarea previa a eliminar (síncrono).
    /// </summary>
    /// <param name="deleteDTO">DTO de entrada.</param>
    /// <param name="record">Entidad a eliminar.</param>
    protected virtual void PrevTask(UserActionDTO<I> deleteDTO, E record) { }

    /// <summary>
    /// Tarea previa a eliminar (asíncrono).
    /// </summary>
    /// <param name="deleteDTO">DTO de entrada.</param>
    /// <param name="record">Entidad a eliminar.</param>
    protected virtual Task PrevTaskAsync(UserActionDTO<I> deleteDTO, E record) => Task.FromResult(Unit.Value);
}
