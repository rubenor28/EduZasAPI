using Application.DAOs;
using Application.DTOs.Common;
using Application.Services;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Caso de uso genérico para actualizar una entidad existente en el sistema.
/// </summary>
/// <typeparam name="UE">Tipo de los datos requeridos para la actualización.</typeparam>
/// <typeparam name="E">Tipo de la entidad resultante.</typeparam>
public abstract class UpdateUseCase<UE, E>(
    IUpdaterAsync<E, UE> updater,
    IBusinessValidationService<UE>? validator = null
)
    where UE : notnull
    where E : notnull
{
    /// <summary>
    /// Ejecuta el proceso de actualización de una entidad, validando los datos de entrada
    /// y aplicando las reglas de negocio correspondientes.
    /// </summary>
    /// <param name="request">Datos de actualización de la entidad.</param>
    /// <param name="formatData">
    /// Función opcional para formatear o transformar los datos de entrada antes de validarlos.
    /// Actualmente no se utiliza en esta implementación.
    /// </param>
    /// <param name="extraValidation">
    /// Función opcional para realizar validaciones sincrónicas adicionales sobre los datos.
    /// Actualmente no se utiliza en esta implementación.
    /// </param>
    /// <param name="extraValidationAsync">
    /// Función opcional para realizar validaciones asincrónicas adicionales sobre los datos.
    /// Actualmente no se utiliza en esta implementación.
    /// </param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> que contiene la entidad actualizada en caso de éxito,
    /// o una lista de <see cref="FieldErrorDTO"/> si las validaciones fallan.
    /// </returns>
    public async Task<Result<E, UseCaseErrorImpl>> ExecuteAsync(UE request)
    {
        var formatted = PreValidationFormat(request);

        if (validator is not null)
        {
            var validation = validator.IsValid(formatted);
            if (validation.IsErr)
                return UseCaseError.Input(validation.UnwrapErr());
        }

        var syncCheck = ExtraValidation(formatted);
        if (syncCheck.IsErr)
            return syncCheck.UnwrapErr();

        var asyncCheck = await ExtraValidationAsync(formatted);
        if (asyncCheck.IsErr)
            return Result<E, UseCaseErrorImpl>.Err(asyncCheck.UnwrapErr());

        formatted = PostValidationFormat(formatted);
        formatted = await PostValidationFormatAsync(formatted);
        var updatedRecord = await updater.UpdateAsync(request);

        ExtraTask(formatted, updatedRecord);
        await ExtraTaskAsync(formatted, updatedRecord);

        return updatedRecord;
    }

    /// <summary>
    /// Aplica formato a los datos antes de la validación principal.
    /// </summary>
    /// <param name="value">Datos a formatear.</param>
    /// <returns>Datos formateados.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para aplicar transformaciones iniciales a los datos.
    /// </remarks>
    protected virtual UE PreValidationFormat(UE value) => value;

    /// <summary>
    /// Aplica formato a los datos después de la validación principal de forma asíncrona.
    /// </summary>
    /// <param name="value">Datos a formatear.</param>
    /// <returns>Datos formateados.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para aplicar transformaciones que requieren operaciones asíncronas.
    /// </remarks>
    protected virtual Task<UE> PostValidationFormatAsync(UE value) => Task.FromResult(value);

    /// <summary>
    /// Aplica formato a los datos después de todas las validaciones.
    /// </summary>
    /// <param name="value">Datos a formatear.</param>
    /// <returns>Datos formateados.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para aplicar transformaciones finales a los datos,
    /// como el hashing de contraseñas antes de la persistencia.
    /// </remarks>
    protected virtual UE PostValidationFormat(UE value) => value;

    /// <summary>
    /// Realiza validaciones adicionales síncronas.
    /// </summary>
    /// <param name="value">Datos a validar.</param>
    /// <returns>Resultado de la validación.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para agregar validaciones personalizadas síncronas.
    /// </remarks>
    protected virtual Result<Unit, UseCaseErrorImpl> ExtraValidation(UE value) =>
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
    protected virtual Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(UE value) =>
        Task.FromResult(Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value));

    /// <summary>
    /// Ejecuta tareas adicionales síncronas después de actualizar la entidad.
    /// </summary>
    /// <param name="newEntity">DTO con los datos originales de la nueva entidad.</param>
    /// <param name="createdEntity">Entidad creada en el sistema.</param>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica adicional después de la creación exitosa.
    /// </remarks>
    protected virtual void ExtraTask(UE newEntity, E createdEntity) { }

    /// <summary>
    /// Ejecuta tareas adicionales asíncronas después de actualizar la entidad.
    /// </summary>
    /// <param name="newEntity">DTO con los datos originales de la nueva entidad.</param>
    /// <param name="createdEntity">Entidad creada en el sistema.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    /// <remarks>
    /// Este método puede ser sobrescrito para ejecutar lógica asíncrona adicional después de la creación exitosa.
    /// </remarks>
    protected virtual Task ExtraTaskAsync(UE newEntity, E createdEntity) =>
        Task.FromResult(Unit.Value);
}
