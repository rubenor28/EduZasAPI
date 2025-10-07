using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Users;

/// <summary>
/// DTO para la eliminación de un usuario.
/// </summary>
/// <remarks>
/// Contiene la información necesaria para procesar la solicitud de eliminación de un usuario,
/// incluyendo el ID del usuario a eliminar y los datos del usuario que ejecuta la operación.
/// </remarks>
public record class DeleteUserDTO : IIdentifiable<ulong>
{
    /// <summary>
    /// Obtiene o establece el ID del usuario que se va a eliminar.
    /// </summary>
    public required ulong Id { get; set; }

    /// <summary>
    /// Obtiene o establece la información del usuario que ejecuta la operación de eliminación.
    /// </summary>
    public required Executor Executor { get; set; }
}
