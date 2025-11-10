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
public abstract class ReadUseCase<I, RE, E>(
    IReaderAsync<I, E> reader,
    IBusinessValidationService<I> validator
) : IUseCaseAsync<RE, E>
    where I : notnull
    where RE : notnull, IIdentifiable<I>
    where E : notnull, IIdentifiable<I>
{
    /// <summary>
    /// Entidad encargada de recuperar una entidad por ID
    /// </summary>
    protected readonly IReaderAsync<I, E> _reader = reader;

    /// <summary>
    /// Entidad encargada de recuperar una entidad por ID
    /// </summary>
    protected readonly IBusinessValidationService<I> _validator = validator;

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
        var validation = _validator.IsValid(request.Id);

        if (validation.IsErr)
        {
            var errors = validation.UnwrapErr();
            return UseCaseErrors.Input(errors);
        }

        var record = await _reader.GetAsync(request.Id);

        if(record.IsNone)
          return UseCaseErrors.NotFound();

        return record.Unwrap();
    }
}
