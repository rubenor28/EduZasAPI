using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Caso de uso genérico para la lectura de una entidad por su identificador.
/// Aplica validación de negocio antes de ejecutar la operación de lectura.
/// </summary>
/// <typeparam name="I">Tipo del identificador de la entidad.</typeparam>
/// <typeparam name="E">Tipo de la entidad que implementa <see cref="IIdentifiable{I}"/>.</typeparam>
public class ReadUseCase<I, E> : IUseCaseAsync<I, Optional<E>, List<FieldErrorDTO>>
    where I : notnull
    where E : notnull, IIdentifiable<I>
{
    private readonly IReaderAsync<I, E> _reader;
    private readonly IBusinessValidationService<I> _validator;

    /// <summary>
    /// Inicializa una nueva instancia del caso de uso de lectura.
    /// </summary>
    /// <param name="reader">Componente encargado de obtener la entidad desde la fuente de datos.</param>
    /// <param name="validator">Servicio de validación de negocio para el identificador.</param>
    public ReadUseCase(IReaderAsync<I, E> reader, IBusinessValidationService<I> validator)
    {
        _reader = reader;
        _validator = validator;
    }

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
    public async Task<Result<Optional<E>, List<FieldErrorDTO>>> ExecuteAsync(I request)
    {
        var validation = _validator.IsValid(request);

        if (validation.IsErr)
        {
            var errors = validation.UnwrapErr();
            return Result<Optional<E>, List<FieldErrorDTO>>.Err(errors);
        }

        var record = await _reader.GetAsync(request);
        return Result<Optional<E>, List<FieldErrorDTO>>.Ok(record);
    }
}
