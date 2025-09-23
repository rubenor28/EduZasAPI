using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Caso de uso genérico para agregar una nueva entidad.
/// </summary>
/// <typeparam name="NE">Tipo de los datos de entrada para la creación.</typeparam>
/// <typeparam name="E">Tipo de la entidad resultante.</typeparam>
public class AddUseCase<NE, E> : IUseCaseAsync<NE, E, List<FieldErrorDTO>>
    where NE : notnull
    where E : notnull
{
    private readonly ICreatorAsync<E, NE> _creator;
    private readonly IBusinessValidationService<NE> _validator;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AddUseCase{NE, E}"/>.
    /// </summary>
    /// <param name="creator">Servicio para crear entidades.</param>
    /// <param name="validator">Servicio para validar reglas de negocio.</param>
    public AddUseCase(ICreatorAsync<E, NE> creator, IBusinessValidationService<NE> validator)
    {
        _creator = creator;
        _validator = validator;
    }

    /// <summary>
    /// Ejecuta la operación de agregar de forma asíncrona.
    /// </summary>
    /// <param name="request">Datos de entrada para la creación de la entidad.</param>
    /// <returns>
    /// Resultado que contiene la entidad creada si fue exitosa,
    /// o una lista de errores de validación si falló.
    /// </returns>
    public async Task<Result<E, List<FieldErrorDTO>>> ExecuteAsync(NE request)
    {
        var validation = _validator.IsValid(request);

        if (validation.IsErr)
        {
            var errors = validation.UnwrapErr();
            return Result<E, List<FieldErrorDTO>>.Err(errors);
        }

        var newRecord = await _creator.AddAsync(request);
        return Result<E, List<FieldErrorDTO>>.Ok(newRecord);
    }
}
