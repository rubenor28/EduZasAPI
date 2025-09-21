namespace EduZasAPI.Application.UseCases.Common;

using EduZasAPI.Domain.ValueObjects.Common;

/// <summary>
/// Define un contrato para casos de uso asíncronos que producen una respuesta.
/// </summary>
/// <typeparam name="TRequest">Tipo de la solicitud. Debe ser no nulo.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta. Debe ser no nulo.</typeparam>
/// <typeparam name="TError">Tipo del error. Debe ser no nulo.</typeparam>
/// <remarks>
/// Esta interfaz se utiliza para implementar casos de uso que ejecutan operaciones asíncronas
/// y devuelven un resultado que puede ser una respuesta exitosa o un error tipado.
/// </remarks>
public interface IUseCaseAsync<TRequest, TResponse, TError>
where TRequest : notnull
where TResponse : notnull
where TError : notnull
{
    /// <summary>
    /// Ejecuta el caso de uso de forma asíncrona con la solicitud proporcionada.
    /// </summary>
    /// <param name="request">Solicitud que contiene los datos necesarios para la ejecución.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado contiene un <see cref="Result{T, E}"/>
    /// con la respuesta exitosa de tipo <typeparamref name="TResponse"/> o un error de tipo <typeparamref name="TError"/>.
    /// </returns>
    Task<Result<TResponse, TError>> ExecuteAsync(TRequest request);
}

/// <summary>
/// Define un contrato para casos de uso síncronos que producen una respuesta.
/// </summary>
/// <typeparam name="TRequest">Tipo de la solicitud. Debe ser no nulo.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta. Debe ser no nulo.</typeparam>
/// <typeparam name="TError">Tipo del error. Debe ser no nulo.</typeparam>
/// <remarks>
/// Esta interfaz se utiliza para implementar casos de uso que ejecutan operaciones síncronas
/// y devuelven un resultado que puede ser una respuesta exitosa o un error tipado.
/// </remarks>
public interface IUseCaseSync<TRequest, TResponse, TError>
where TRequest : notnull
where TResponse : notnull
where TError : notnull
{
    /// <summary>
    /// Ejecuta el caso de uso de forma síncrona con la solicitud proporcionada.
    /// </summary>
    /// <param name="request">Solicitud que contiene los datos necesarios para la ejecución.</param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> con la respuesta exitosa de tipo <typeparamref name="TResponse"/>
    /// o un error de tipo <typeparamref name="TError"/>.
    /// </returns>
    Result<TResponse, TError> Execute(TRequest request);
}
