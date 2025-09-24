using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Caso de uso genérico para actualizar una entidad existente en el sistema.
/// </summary>
/// <typeparam name="UE">Tipo de los datos requeridos para la actualización.</typeparam>
/// <typeparam name="E">Tipo de la entidad resultante.</typeparam>
public class UpdateUseCase<UE, E>
    where UE : notnull
    where E : notnull
{
    private readonly IUpdaterAsync<E, UE> _updater;
    private readonly IBusinessValidationService<UE> _validator;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="UpdateUseCase{UE, E}"/>.
    /// </summary>
    /// <param name="updater">Servicio encargado de aplicar las actualizaciones sobre la entidad.</param>
    /// <param name="validator">Servicio para validar reglas de negocio sobre los datos de actualización.</param>
    public UpdateUseCase(IUpdaterAsync<E, UE> updater, IBusinessValidationService<UE> validator)
    {
        _updater = updater;
        _validator = validator;
    }

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
    public async Task<Result<E, List<FieldErrorDTO>>> ExecuteAsync(
        UE request,
        Func<UE, UE>? formatData = null,
        Func<UE, Result<Unit, List<FieldErrorDTO>>>? extraValidation = null,
        Func<UE, Task<Result<Unit, List<FieldErrorDTO>>>>? extraValidationAsync = null)
    {
        var validation = _validator.IsValid(request);

        if (validation.IsErr)
        {
            var errors = validation.UnwrapErr();
            return Result<E, List<FieldErrorDTO>>.Err(errors);
        }

        var updatedRecord = await _updater.UpdateAsync(request);
        return Result<E, List<FieldErrorDTO>>.Ok(updatedRecord);
    }
}
