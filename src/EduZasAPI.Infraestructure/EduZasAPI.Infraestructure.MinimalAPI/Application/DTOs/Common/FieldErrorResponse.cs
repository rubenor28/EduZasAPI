using EduZasAPI.Application.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

/// <summary>
/// Representa la respuesta estándar que se devuelve cuando ocurre
/// un error de validación o de negocio en la API.
/// </summary>
public class FieldErrorResponse
{
    /// <summary>
    /// Mensaje general que describe el error (ej. "Validación fallida").
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Lista de errores específicos por campo que detallan
    /// las causas del fallo.
    /// </summary>
    public required List<FieldErrorDTO> Errors { get; init; }
}
