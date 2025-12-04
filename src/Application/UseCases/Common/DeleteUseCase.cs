using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.Services;
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

    /// <summary>
    /// Ejecuta el caso de uso para eliminar una entidad.
    /// </summary>
    /// <param name="request">El DTO con la información para la eliminación.</param>
    /// <returns>Un <see cref="Result{T, E}"/> que contiene la entidad eliminada o un error.</returns>
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

    protected virtual Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<I> value,
        E record
    ) => Result<Unit, UseCaseError>.Ok(Unit.Value);

    protected virtual async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<I> value,
        E record
    ) => Unit.Value;

    protected virtual void ExtraTask(UserActionDTO<I> deleteDTO, E deletedEntity) { }

    protected virtual Task ExtraTaskAsync(UserActionDTO<I> deleteDTO, E deletedEntity) =>
        Task.FromResult(Unit.Value);

    protected virtual void PrevTask(UserActionDTO<I> deleteDTO, E record) { }

    protected virtual Task PrevTaskAsync(UserActionDTO<I> deleteDTO, E record) => Task.FromResult(Unit.Value);
}
