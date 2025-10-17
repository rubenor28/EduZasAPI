using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Define un contrato para casos de uso asíncronos que producen una respuesta
/// y un error tipado en caso de fallo.
/// </summary>
/// <typeparam name="TRequest">Tipo de la solicitud. Debe ser no nulo.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta. Debe ser no nulo.</typeparam>
public interface IUseCaseAsync<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    /// <summary>
    /// Ejecuta el caso de uso de forma asíncrona con la solicitud proporcionada.
    /// </summary>
    /// <param name="request">Solicitud con los datos necesarios para la ejecución.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona.
    /// Contiene un resultado que puede ser exitoso con <typeparamref name="TResponse"/>
    /// o fallido con <typeparamref name="TError"/>.
    /// </returns>
    Task<Result<TResponse, UseCaseErrorImpl>> ExecuteAsync(TRequest request);
}

/// <summary>
/// Define un contrato para casos de uso asíncronos que producen una respuesta.
/// </summary>
/// <typeparam name="TRequest">Tipo de la solicitud. Debe ser no nulo.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta. Debe ser no nulo.</typeparam>
public interface IGuaranteedUseCaseAsync<TRequest, TResponse>
{
    /// <summary>
    /// Ejecuta el caso de uso de forma asíncrona con la solicitud proporcionada.
    /// </summary>
    /// <param name="request">Solicitud con los datos necesarios para la ejecución.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona.
    /// Contiene un resultado <typeparamref name="TResponse"/>.
    /// </returns>
    Task<TResponse> ExecuteAsync(TRequest request);
}
