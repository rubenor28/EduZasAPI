namespace EduZasAPI.Application.UseCases.Common;

using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Application.Ports.DAOs;
using EduZasAPI.Application.Ports.Services.Common;
using EduZasAPI.Application.DTOs.Common;

/// <summary>
/// Caso de uso genérico para actualizar una entidad existente.
/// </summary>
/// <typeparam name="UE">Tipo de los datos de actualización.</typeparam>
/// <typeparam name="E">Tipo de la entidad resultante.</typeparam>
public class UpdateUseCase<UE, E> : IUseCaseAsync<UE, E, List<FieldErrorDTO>>
    where UE : notnull
    where E : notnull
{
    private readonly IUpdaterAsync<E, UE> _updater;
    private readonly IBusinessValidationService<UE> _validator;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="UpdateUseCase{UE, E}"/>.
    /// </summary>
    /// <param name="updater">Servicio para actualizar entidades.</param>
    /// <param name="validator">Servicio para validar reglas de negocio sobre los datos de actualización.</param>
    public UpdateUseCase(IUpdaterAsync<E, UE> updater, IBusinessValidationService<UE> validator)
    {
        _updater = updater;
        _validator = validator;
    }

    /// <summary>
    /// Ejecuta la operación de actualización de forma asíncrona.
    /// </summary>
    /// <param name="request">Datos para actualizar la entidad.</param>
    /// <returns>
    /// Resultado que contiene la entidad actualizada si fue exitosa,
    /// o una lista de errores de validación si falló.
    /// </returns>
    public async Task<Result<E, List<FieldErrorDTO>>> ExecuteAsync(UE request)
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
