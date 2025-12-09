using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Interfaz base para casos de uso asíncronos que requieren contexto de usuario.
/// </summary>
/// <typeparam name="TRequest">Tipo de datos de entrada.</typeparam>
/// <typeparam name="TResponse">Tipo de datos de respuesta.</typeparam>
public interface IUseCaseAsync<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    /// <summary>
    /// Ejecuta la lógica del caso de uso.
    /// </summary>
    /// <param name="request">DTO con los datos de entrada y el ejecutor.</param>
    /// <returns>Resultado de la operación o error.</returns>
    Task<Result<TResponse, UseCaseError>> ExecuteAsync(UserActionDTO<TRequest> request);
}

/// <summary>
/// Interfaz base para casos de uso asíncronos que no requieren contexto de usuario (invitado).
/// </summary>
/// <typeparam name="TRequest">Tipo de datos de entrada.</typeparam>
/// <typeparam name="TResponse">Tipo de datos de respuesta.</typeparam>
public interface IGuestUseCaseAsync<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    /// <summary>
    /// Ejecuta la lógica del caso de uso.
    /// </summary>
    /// <param name="request">Datos de entrada.</param>
    /// <returns>Resultado de la operación o error.</returns>
    Task<Result<TResponse, UseCaseError>> ExecuteAsync(TRequest request);
}
