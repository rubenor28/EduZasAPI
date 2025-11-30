using Application.DAOs;
using Application.DTOs.Common;
using Application.Services;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Caso de uso genérico para la lectura de una entidad por su identificador.
/// Aplica validación de negocio antes de ejecutar la operación de lectura.
/// </summary>
/// <typeparam name="I">Tipo del identificador de la entidad.</typeparam>
/// <typeparam name="E">Tipo de la entidad que implementa <see cref="IIdentifiable{I}"/>.</typeparam>
[Obsolete("Usar ReadUseCase<I,RE,E> en su lugar")]
public abstract class ReadUseCase<I, E>(
    IReaderAsync<I, E> reader,
    IBusinessValidationService<I>? validator = null
) : IUseCaseAsync<I, E>
    where I : notnull
    where E : notnull
{
    /// <summary>
    /// Entidad encargada de recuperar una entidad por ID
    /// </summary>
    protected readonly IReaderAsync<I, E> _reader = reader;

    /// <summary>
    /// Entidad encargada de validar la entrada brindada
    /// </summary>
    protected readonly IBusinessValidationService<I>? _validator = validator;

    /// <summary>
    /// Ejecuta la operación de lectura validando previamente el identificador.
    /// </summary>
    /// <param name="request">Identificador de la entidad a consultar.</param>
    /// <returns>
    /// Un <see cref="Result{TResponse, TError}"/> que contiene:
    /// <list type="bullet">
    /// <item><description>La entidad envuelta en un <see cref="Optional{T}"/> si la operación es exitosa.</description></item>
    /// <item><description>Una lista de errores de validación si la operación falla.</description></item>
    /// </list>
    /// </returns>
    public async Task<Result<E, UseCaseError>> ExecuteAsync(I request)
    {
        if (_validator is not null)
        {
            var validation = _validator.IsValid(request);

            if (validation.IsErr)
            {
                var errors = validation.UnwrapErr();
                return UseCaseErrors.Input(errors);
            }
        }

        var record = await _reader.GetAsync(request);

        if (record.IsNone)
            return UseCaseErrors.NotFound();

        return record.Unwrap();
    }
}

public abstract class ReadUseCase<I, RE, E>(
    IReaderAsync<I, E> reader,
    IBusinessValidationService<RE>? validator = null
)
    where I : notnull
    where E : notnull
    where RE : notnull
{
    /// <summary>
    /// Entidad encargada de recuperar una entidad por ID
    /// </summary>
    private readonly IReaderAsync<I, E> _reader = reader;

    /// <summary>
    /// Entidad encargada de validar la entrada brindada
    /// </summary>
    private readonly IBusinessValidationService<RE>? _validator = validator;

    /// <summary>
    /// Ejecuta la operación de lectura validando previamente el identificador.
    /// </summary>
    /// <param name="request">Identificador de la entidad a consultar.</param>
    /// <returns>
    /// Un <see cref="Result{TResponse, TError}"/> que contiene:
    /// <list type="bullet">
    /// <item><description>La entidad envuelta en un <see cref="Optional{T}"/> si la operación es exitosa.</description></item>
    /// <item><description>Una lista de errores de validación si la operación falla.</description></item>
    /// </list>
    /// </returns>
    public async Task<Result<E, UseCaseError>> ExecuteAsync(RE request)
    {
        if (_validator is not null)
        {
            var validation = _validator.IsValid(request);

            if (validation.IsErr)
            {
                var errors = validation.UnwrapErr();
                return UseCaseErrors.Input(errors);
            }
        }

        var syncCheck = ExtraValidation(request);
        if (syncCheck.IsErr)
            return syncCheck.UnwrapErr();

        var asyncCheck = await ExtraValidationAsync(request);
        if (syncCheck.IsErr)
            return asyncCheck.UnwrapErr();

        PrevTask(request);
        await PrevTaskAsync(request);

        var record = await _reader.GetAsync(GetId(request));

        if (record.IsNone)
            return UseCaseErrors.NotFound();

        ExtraTask(data, recordDeleted);
        await ExtraTaskAsync(data, recordDeleted);

        return record.Unwrap();
    }

    /// <summary>
    /// Realiza validaciones adicionales síncronas.
    /// </summary>
    /// <param name="value">Datos a validar.</param>
    /// <returns>Resultado de la validación.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para agregar validaciones personalizadas síncronas.
    /// </remarks>
    protected virtual Result<Unit, UseCaseError> ExtraValidation(RE value) =>
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
    protected async virtual Task<Result<Unit, UseCaseError>> ExtraValidationAsync(RE value)
    {
        var record = await _reader.GetAsync(GetId(value));
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
    protected virtual void ExtraTask(RE deleteDTO, E deletedEntity) { }

    /// <summary>
    /// Ejecuta tareas adicionales asíncronas después de eliminar la entidad.
    /// </summary>
    /// <param name="deleteDTOewEntity">DTO con los datos originales de la nueva entidad.</param>
    /// <param name="deletedEntity">Entidad creada en el sistema.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica asíncrona adicional después de la creación exitosa.
    /// </remarks>
    protected virtual Task ExtraTaskAsync(RE deleteDTO, E deletedEntity) =>
        Task.FromResult(Unit.Value);

    /// <summary>
    /// Ejecuta tareas adicionales síncronas previas a eliminar la entidad.
    /// </summary>
    /// <param name="deleteDTO">DTO con los datos originales de la nueva entidad.</param>
    /// <param name="deletedEntity">Entidad creada en el sistema.</param>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica adicional después de la creación exitosa.
    /// </remarks>
    protected virtual void PrevTask(RE deleteDTO) { }

    /// <summary>
    /// Ejecuta tareas adicionales asíncronas previas a eliminar la entidad.
    /// </summary>
    /// <param name="deleteDTO">DTO con los datos originales de la nueva entidad.</param>
    /// <param name="deletedEntity">Entidad creada en el sistema.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica asíncrona adicional después de la creación exitosa.
    /// </remarks>
    protected virtual Task PrevTaskAsync(RE deleteDTO) => Task.FromResult(Unit.Value);

    public abstract I GetId(RE request);
}
