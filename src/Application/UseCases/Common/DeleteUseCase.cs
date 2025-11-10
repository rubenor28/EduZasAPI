using Application.DAOs;
using Application.DTOs.Common;
using Application.Services;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Caso de uso genérico para eliminar una entidad del sistema.
/// </summary>
/// <typeparam name="I">El tipo del identificador de la entidad.</typeparam>
/// <typeparam name="DE">El tipo del DTO de entrada para la eliminación.</typeparam>
/// <typeparam name="E">El tipo de la entidad de dominio que se eliminará.</typeparam>
public abstract class DeleteUseCase<I, DE, E>(
    IDeleterAsync<I, E> deleter,
    IReaderAsync<I, E> reader,
    IBusinessValidationService<DE>? validator = null
) : IUseCaseAsync<DE, E>
    where I : notnull
    where E : notnull, IIdentifiable<I>
    where DE : notnull, IIdentifiable<I>
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
    protected readonly IBusinessValidationService<DE>? _validator = validator;

    /// <summary>
    /// Ejecuta el caso de uso para eliminar una entidad.
    /// </summary>
    /// <param name="data">El DTO con la información para la eliminación.</param>
    /// <returns>Un <see cref="Result{T, E}"/> que contiene la entidad eliminada o un error.</returns>
    public async Task<Result<E, UseCaseError>> ExecuteAsync(DE data)
    {
        if (_validator is not null)
        {
            var validation = _validator.IsValid(data);
            if (validation.IsErr)
                return UseCaseErrors.Input(validation.UnwrapErr());
        }

        var syncCheck = ExtraValidation(data);
        if (syncCheck.IsErr)
            return syncCheck.UnwrapErr();

        var asyncCheck = await ExtraValidationAsync(data);
        if (asyncCheck.IsErr)
            return asyncCheck.UnwrapErr();

        PrevTask(data);
        await PrevTaskAsync(data);

        var recordDeleted = await _deleter.DeleteAsync(data.Id);

        ExtraTask(data, recordDeleted);
        await ExtraTaskAsync(data, recordDeleted);
        return recordDeleted;
    }

    /// <summary>
    /// Realiza validaciones adicionales síncronas.
    /// </summary>
    /// <param name="value">Datos a validar.</param>
    /// <returns>Resultado de la validación.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para agregar validaciones personalizadas síncronas.
    /// </remarks>
    protected virtual Result<Unit, UseCaseError> ExtraValidation(DE value) =>
        Result<Unit, UseCaseError>.Ok(Unit.Value);

    /// <summary>
    /// Realiza validaciones adicionales asíncronas.
    /// </summary>
    /// <param name="value">Datos a validar.</param>
    /// <returns>Tarea que representa la validación asíncrona.</returns>
    /// <remarks>
    /// Por defecto busca la existencia por ID del registro y retorna un <see cref="NotFoundError">
    /// si no se encuentra
    ///
    /// Este método puede ser sobrescrito para agregar validaciones personalizadas asíncronas,
    /// como verificaciones en base de datos o llamadas a servicios externos.
    /// </remarks>
    protected async virtual Task<Result<Unit, UseCaseError>> ExtraValidationAsync(DE value)
    {
        var record = await _reader.GetAsync(value.Id);
        if (record.IsNone)
            return UseCaseErrors.NotFound();

        return Unit.Value;
    }

    /// <summary>
    /// Ejecuta tareas adicionales síncronas después de eliminar la entidad.
    /// </summary>
    /// <param name="deleteDTO">DTO con los datos originales de la nueva entidad.</param>
    /// <param name="deletedEntity">Entidad creada en el sistema.</param>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica adicional después de la creación exitosa.
    /// </remarks>
    protected virtual void ExtraTask(DE deleteDTO, E deletedEntity) { }

    /// <summary>
    /// Ejecuta tareas adicionales asíncronas después de eliminar la entidad.
    /// </summary>
    /// <param name="deleteDTOewEntity">DTO con los datos originales de la nueva entidad.</param>
    /// <param name="deletedEntity">Entidad creada en el sistema.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica asíncrona adicional después de la creación exitosa.
    /// </remarks>
    protected virtual Task ExtraTaskAsync(DE deleteDTO, E deletedEntity) =>
        Task.FromResult(Unit.Value);

    /// <summary>
    /// Ejecuta tareas adicionales síncronas previas a eliminar la entidad.
    /// </summary>
    /// <param name="deleteDTO">DTO con los datos originales de la nueva entidad.</param>
    /// <param name="deletedEntity">Entidad creada en el sistema.</param>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica adicional después de la creación exitosa.
    /// </remarks>
    protected virtual void PrevTask(DE deleteDTO) { }

    /// <summary>
    /// Ejecuta tareas adicionales asíncronas previas a eliminar la entidad.
    /// </summary>
    /// <param name="deleteDTO">DTO con los datos originales de la nueva entidad.</param>
    /// <param name="deletedEntity">Entidad creada en el sistema.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica asíncrona adicional después de la creación exitosa.
    /// </remarks>
    protected virtual Task PrevTaskAsync(DE deleteDTO) => Task.FromResult(Unit.Value);
}
