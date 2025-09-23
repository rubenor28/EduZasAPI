using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Define un contrato para casos de uso asíncronos que producen una respuesta
/// y un error tipado en caso de fallo.
/// </summary>
/// <typeparam name="TRequest">Tipo de la solicitud. Debe ser no nulo.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta. Debe ser no nulo.</typeparam>
/// <typeparam name="TError">Tipo del error. Debe ser no nulo.</typeparam>
public interface IUseCaseAsync<TRequest, TResponse, TError>
    where TRequest : notnull
    where TResponse : notnull
    where TError : notnull
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
    Task<Result<TResponse, TError>> ExecuteAsync(TRequest request);
}

/// <summary>
/// Define un contrato para casos de uso asíncronos que producen únicamente una respuesta.
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
    /// Contiene la respuesta de tipo <typeparamref name="TResponse"/>.
    /// </returns>
    Task<TResponse> ExecuteAsync(TRequest request);
}

/// <summary>
/// Define un contrato para casos de uso síncronos que producen una respuesta
/// y un error tipado en caso de fallo.
/// </summary>
/// <typeparam name="TRequest">Tipo de la solicitud. Debe ser no nulo.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta. Debe ser no nulo.</typeparam>
/// <typeparam name="TError">Tipo del error. Debe ser no nulo.</typeparam>
public interface IUseCaseSync<TRequest, TResponse, TError>
    where TRequest : notnull
    where TResponse : notnull
    where TError : notnull
{
    /// <summary>
    /// Ejecuta el caso de uso de forma síncrona con la solicitud proporcionada.
    /// </summary>
    /// <param name="request">Solicitud con los datos necesarios para la ejecución.</param>
    /// <returns>
    /// Un resultado que puede ser exitoso con <typeparamref name="TResponse"/>
    /// o fallido con <typeparamref name="TError"/>.
    /// </returns>
    Result<TResponse, TError> Execute(TRequest request);
}

/// <summary>
/// Define un contrato para casos de uso síncronos que producen únicamente una respuesta.
/// </summary>
/// <typeparam name="TRequest">Tipo de la solicitud. Debe ser no nulo.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta. Debe ser no nulo.</typeparam>
public interface IUseCaseSync<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    /// <summary>
    /// Ejecuta el caso de uso de forma síncrona con la solicitud proporcionada.
    /// </summary>
    /// <param name="request">Solicitud con los datos necesarios para la ejecución.</param>
    /// <returns>
    /// La respuesta de tipo <typeparamref name="TResponse"/>.
    /// </returns>
    TResponse Execute(TRequest request);
}
