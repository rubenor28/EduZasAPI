using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Caso de uso base para la adición de nuevas entidades con validación extensible.
/// </summary>
/// <typeparam name="NE">Tipo del DTO para nueva entidad. Debe ser no nulo.</typeparam>
/// <typeparam name="E">Tipo de la entidad resultante. Debe ser no nulo.</typeparam>
/// <remarks>
/// Esta clase proporciona una implementación base para casos de uso que crean nuevas entidades,
/// incluyendo validación en múltiples etapas, formato de datos y extensibilidad mediante hooks.
/// </remarks>
public class AddUseCase<NE, E> : IUseCaseAsync<NE, Result<E, UseCaseErrorImpl>>
    where NE : notnull
    where E : notnull
{
    /// <summary>
    /// Creador responsable de persistir la nueva entidad.
    /// </summary>
    protected readonly ICreatorAsync<E, NE> _creator;

    /// <summary>
    /// Validador de reglas de negocio para la entidad.
    /// </summary>
    protected readonly IBusinessValidationService<NE>? _validator;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="AddUseCase{NE, E}"/>.
    /// </summary>
    /// <param name="creator">Implementación del creador de entidades.</param>
    /// <param name="validator">Implementación del validador de reglas de negocio.</param>
    public AddUseCase(ICreatorAsync<E, NE> creator, IBusinessValidationService<NE>? validator = null)
    {
        _creator = creator;
        _validator = validator;
    }

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
                var err = UseCaseError.Input(validation.UnwrapErr());
                return Result<E, UseCaseErrorImpl>.Err(err);
            }
        }

        var syncCheck = ExtraValidation(formatted);
        if (syncCheck.IsErr)
            return Result<E, UseCaseErrorImpl>.Err(syncCheck.UnwrapErr());

        var asyncCheck = await ExtraValidationAsync(formatted);
        if (asyncCheck.IsErr)
            return Result<E, UseCaseErrorImpl>.Err(asyncCheck.UnwrapErr());

        formatted = PostValidationFormat(formatted);
        formatted = await PostValidationFormatAsync(formatted);
        var newRecord = await _creator.AddAsync(formatted);

        ExtraTask(formatted, newRecord);
        await ExtraTaskAsync(formatted, newRecord);

        return Result<E, UseCaseErrorImpl>.Ok(newRecord);
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
    protected virtual Task ExtraTaskAsync(NE newEntity, E createdEntity) => Task.FromResult(Unit.Value);
}
