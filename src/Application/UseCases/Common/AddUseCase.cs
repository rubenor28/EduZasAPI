using Application.DAOs;
using Application.DTOs.Common;
using Application.Services;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Caso de uso base para la adición de nuevas entidades con validación extensible.
/// </summary>
/// <typeparam name="NE">Tipo del DTO para nueva entidad. Debe ser no nulo.</typeparam>
/// <typeparam name="E">Tipo de la entidad resultante. Debe ser no nulo.</typeparam>
/// <remarks>
/// Esta clase proporciona una implementación base para casos de uso que crean nuevas entidades,
/// incluyendo validación en múltiples etapas, formato de datos y extensibilidad mediante hooks.
/// </remarks>
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

    /// <summary>
    /// Ejecuta el proceso completo de creación de una nueva entidad.
    /// </summary>
    /// <param name="request">DTO con los datos para crear la nueva entidad.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado contiene la entidad creada
    /// o una lista de errores de validación si el proceso falla.
    /// </returns>
    public async virtual Task<Result<E, UseCaseErrorImpl>> ExecuteAsync(NE request)
    {
        var formatted = PreValidationFormat(request);

        if (_validator is not null)
        {
            var validation = _validator.IsValid(formatted);
            if (validation.IsErr)
            {
                return UseCaseError.Input(validation.UnwrapErr());
            }
        }

        var syncCheck = ExtraValidation(formatted);
        if (syncCheck.IsErr)
            return syncCheck.UnwrapErr();

        var asyncCheck = await ExtraValidationAsync(formatted);
        if (asyncCheck.IsErr)
            return asyncCheck.UnwrapErr();

        formatted = PostValidationFormat(formatted);
        formatted = await PostValidationFormatAsync(formatted);

        PrevTask(formatted);
        await PrevTaskAsync(formatted);

        var newRecord = await _creator.AddAsync(formatted);

        ExtraTask(formatted, newRecord);
        await ExtraTaskAsync(formatted, newRecord);

        return newRecord;
    }

    /// <summary>
    /// Aplica formato a los datos antes de la validación principal.
    /// </summary>
    /// <param name="value">Datos a formatear.</param>
    /// <returns>Datos formateados.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para aplicar transformaciones iniciales a los datos.
    /// </remarks>
    protected virtual NE PreValidationFormat(NE value) => value;

    /// <summary>
    /// Aplica formato a los datos después de la validación principal de forma asíncrona.
    /// </summary>
    /// <param name="value">Datos a formatear.</param>
    /// <returns>Datos formateados.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para aplicar transformaciones que requieren operaciones asíncronas.
    /// </remarks>
    protected virtual Task<NE> PostValidationFormatAsync(NE value) => Task.FromResult(value);

    /// <summary>
    /// Aplica formato a los datos después de todas las validaciones.
    /// </summary>
    /// <param name="value">Datos a formatear.</param>
    /// <returns>Datos formateados.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para aplicar transformaciones finales a los datos,
    /// como el hashing de contraseñas antes de la persistencia.
    /// </remarks>
    protected virtual NE PostValidationFormat(NE value) => value;

    /// <summary>
    /// Realiza validaciones adicionales síncronas.
    /// </summary>
    /// <param name="value">Datos a validar.</param>
    /// <returns>Resultado de la validación.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para agregar validaciones personalizadas síncronas.
    /// </remarks>
    protected virtual Result<Unit, UseCaseErrorImpl> ExtraValidation(NE value) =>
        Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);

    /// <summary>
    /// Realiza validaciones adicionales asíncronas.
    /// </summary>
    /// <param name="value">Datos a validar.</param>
    /// <returns>Tarea que representa la validación asíncrona.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para agregar validaciones personalizadas asíncronas,
    /// como verificaciones en base de datos o llamadas a servicios externos.
    /// </remarks>
    protected virtual Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(NE value) =>
        Task.FromResult(Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value));

    /// <summary>
    /// Ejecuta tareas adicionales síncronas después de crear la entidad.
    /// </summary>
    /// <param name="newEntity">DTO con los datos originales de la nueva entidad.</param>
    /// <param name="createdEntity">Entidad creada en el sistema.</param>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica adicional después de la creación exitosa.
    /// </remarks>
    protected virtual void ExtraTask(NE newEntity, E createdEntity) { }

    /// <summary>
    /// Ejecuta tareas adicionales asíncronas después de crear la entidad.
    /// </summary>
    /// <param name="newEntity">DTO con los datos originales de la nueva entidad.</param>
    /// <param name="createdEntity">Entidad creada en el sistema.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica asíncrona adicional después de la creación exitosa.
    /// </remarks>
    protected virtual Task ExtraTaskAsync(NE newEntity, E createdEntity) => Task.CompletedTask;

    /// <summary>
    /// Ejecuta tareas adicionales síncronas prvias a crear la entidad.
    /// </summary>
    /// <param name="newEntity">DTO con los datos originales de la nueva entidad.</param>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica adicional previo a la creación exitosa.
    /// </remarks>
    protected virtual void PrevTask(NE newEntity) { }

    /// <summary>
    /// Ejecuta tareas adicionales asíncronas previo a crear la entidad.
    /// </summary>
    /// <param name="newEntity">DTO con los datos originales de la nueva entidad.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica asíncrona adicional previo a la creación exitosa.
    /// </remarks>
    protected virtual Task PrevTaskAsync(NE newEntity) => Task.CompletedTask;
}
