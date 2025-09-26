using EduZasAPI.Domain.Users;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

/// <summary>
/// Representa la carga útil de autenticación utilizada para generar y validar tokens JWT.
/// </summary>
public class AuthPayload
{
    /// <summary>
    /// Identificador único del usuario autenticado.
    /// </summary>
    /// <value>Un valor <see cref="ulong"/> que representa el ID del usuario.</value>
    public required ulong Id { get; init; }

    /// <summary>
    /// Rol del usuario autenticado en el sistema.
    /// </summary>
    /// <value>Un valor de <see cref="UserType"/> que indica el rol asignado al usuario.</value>
    public required UserType Role { get; set; }
}
