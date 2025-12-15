namespace MinimalAPI.Application.DTOs;

/// <summary>
/// Representa una respuesta de la API que incluye un mensaje y datos de tipo <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">El tipo de los datos devueltos en la respuesta.</typeparam>
public sealed record WithDataResponse<T>
{
    /// <summary>
    /// Mensaje que describe el resultado de la operación.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Datos asociados al resultado de la operación.
    /// </summary>
    public required T Data { get; init; }
}
