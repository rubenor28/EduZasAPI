using Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs;

/// <summary>
/// Representa la respuesta estándar que se devuelve cuando ocurre
/// un error de validación o de negocio en la API.
/// </summary>
public sealed record FieldErrorResponse
{
    /// <summary>
    /// Mensaje general que describe el error (ej. "Validación fallida").
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Lista de errores específicos por campo que detallan
    /// las causas del fallo.
    /// </summary>
    public required IEnumerable<FieldErrorDTO> Errors { get; init; }
}
