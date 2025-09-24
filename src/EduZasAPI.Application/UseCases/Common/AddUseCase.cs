using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Caso de uso genérico para agregar una nueva entidad al sistema.
/// </summary>
/// <typeparam name="NE">Tipo de los datos de entrada requeridos para la creación.</typeparam>
/// <typeparam name="E">Tipo de la entidad resultante.</typeparam>
public class AddUseCase<NE, E>
    where NE : notnull
    where E : notnull
{
    private readonly ICreatorAsync<E, NE> _creator;
    private readonly IBusinessValidationService<NE> _validator;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="AddUseCase{NE, E}"/>.
    /// </summary>
    /// <param name="creator">Servicio responsable de crear y persistir entidades.</param>
    /// <param name="validator">Servicio para validar las reglas de negocio sobre los datos de entrada.</param>
    public AddUseCase(ICreatorAsync<E, NE> creator, IBusinessValidationService<NE> validator)
    {
        _creator = creator;
        _validator = validator;
    }

    /// <summary>
    /// Ejecuta el proceso de creación de una nueva entidad, aplicando validaciones
    /// y transformaciones opcionales a los datos de entrada.
    /// </summary>
    /// <param name="request">Datos de entrada para crear la entidad.</param>
    /// <param name="formatData">
    /// Función opcional para formatear o transformar los datos de entrada antes de las validaciones.
    /// Si no se proporciona, los datos se utilizan sin modificaciones.
    /// </param>
    /// <param name="extraValidation">
    /// Función opcional para ejecutar validaciones sincrónicas adicionales sobre los datos formateados.
    /// Debe devolver un <see cref="Result{T, E}"/> indicando éxito o errores de validación.
    /// </param>
    /// <param name="extraValidationAsync">
    /// Función opcional para ejecutar validaciones asincrónicas adicionales sobre los datos formateados.
    /// Debe devolver un <see cref="Result{T, E}"/> indicando éxito o errores de validación.
    /// </param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> que contiene la entidad creada en caso de éxito,
    /// o una lista de <see cref="FieldErrorDTO"/> con los errores encontrados durante las validaciones.
    /// </returns>
    public async Task<Result<E, List<FieldErrorDTO>>> ExecuteAsync(
        NE request,
        Func<NE, NE>? formatData = null,
        Func<NE, Result<Unit, List<FieldErrorDTO>>>? extraValidation = null,
        Func<NE, Task<Result<Unit, List<FieldErrorDTO>>>>? extraValidationAsync = null)
    {
        var formatted = (formatData ?? (x => x))(request);

        var validation = _validator.IsValid(formatted);
        if (validation.IsErr)
            return Result<E, List<FieldErrorDTO>>.Err(validation.UnwrapErr());

        if (extraValidation is not null)
        {
            var syncCheck = extraValidation(formatted);
            if (syncCheck.IsErr)
                return Result<E, List<FieldErrorDTO>>.Err(syncCheck.UnwrapErr());
        }

        if (extraValidationAsync is not null)
        {
            var asyncCheck = await extraValidationAsync(formatted);
            if (asyncCheck.IsErr)
                return Result<E, List<FieldErrorDTO>>.Err(asyncCheck.UnwrapErr());
        }

        var newRecord = await _creator.AddAsync(formatted);
        return Result<E, List<FieldErrorDTO>>.Ok(newRecord);
    }
}
