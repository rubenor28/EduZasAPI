using EduZasAPI.Domain.Users;

namespace EduZasAPI.Application.Users;

/// <summary>
/// Representa una solicitud de cambio de rol para un usuario existente.
/// </summary>
/// <remarks>
/// Esta estructura inmutable encapsula la información mínima y obligatoria
/// requerida para modificar el rol de un usuario en el sistema.
/// Utiliza campos requeridos (required) para garantizar que tanto el identificador
/// como el nuevo rol estén siempre presentes durante la operación de cambio.
/// </remarks>
public struct RolChangeDTO
{
    /// <summary>
    /// Obtiene o establece el identificador único del usuario cuyo rol se desea modificar.
    /// </summary>
    /// <value>Identificador numérico del usuario. Campo obligatorio.</value>
    public required ulong Id { get; set; }

    /// <summary>
    /// Obtiene o establece el nuevo rol que será asignado al usuario.
    /// </summary>
    /// <value>
    /// Nuevo tipo de usuario definido por <see cref="UserType"/>. Campo obligatorio.
    /// </value>
    public required UserType Role { get; set; }
}
