using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Classes;

/// <summary>
/// DTO para la eliminación de un usuario.
/// </summary>
/// <remarks>
/// Contiene la información necesaria para procesar la solicitud de eliminación de una clase,
/// incluyendo el ID de la clase a eliminar y los datos del usuario que ejecuta la operación.
/// </remarks>
public sealed record DeleteClassDTO : IIdentifiable<string>
{
    /// <summary>
    /// Obtiene o establece el ID de la clase que se va a eliminar.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Obtiene o establece la información del usuario que ejecuta la operación de eliminación.
    /// </summary>
    public required Executor Executor { get; set; }
}
