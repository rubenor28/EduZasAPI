namespace EduZasAPI.Application.UseCases.Users;

using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Application.Ports.DAOs;
using EduZasAPI.Application.Ports.Services.Common;
using EduZasAPI.Application.DTOs.Common;
using EduZasAPI.Application.UseCases.Common;

/// <summary>
/// Caso de uso genérico para agregar una nueva entidad.
/// </summary>
/// <typeparam name="T">Tipo de los datos de entrada para la creación.</typeparam>
/// <typeparam name="U">Tipo de la entidad resultante.</typeparam>
public class AddUseCase<T, U> : IUseCaseAsync<T, U, List<FieldErrorDTO>>
    where T : notnull
    where U : notnull
{
    private readonly ICreatorAsync<U, T> _creator;
    private readonly IBusinessValidationService<T> _validator;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AddUseCase{T, U}"/>.
    /// </summary>
    /// <param name="creator">Servicio para crear entidades.</param>
    /// <param name="validator">Servicio para validar reglas de negocio.</param>
    public AddUseCase(ICreatorAsync<U, T> creator, IBusinessValidationService<T> validator)
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
    public async Task<Result<U, List<FieldErrorDTO>>> ExecuteAsync(T request)
    {
        var validation = _validator.IsValid(request);

        if (validation.IsErr)
        {
            var errors = validation.UnwrapErr();
            return Result<U, List<FieldErrorDTO>>.Err(errors);
        }

        var newRecord = await _creator.AddAsync(request);
        return Result<U, List<FieldErrorDTO>>.Ok(newRecord);
    }
}
