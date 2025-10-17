using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Representa un usuario completo del sistema con toda su información y metadatos.
/// </summary>
/// <remarks>
/// Esta clase implementa la interfaz <see cref="IIdentifiable{T}"/> y encapsula
/// todos los datos de un usuario, incluyendo información personal, credenciales,
/// estado, rol y metadatos de auditoría. Utiliza campos requeridos para garantizar
/// la integridad de los datos esenciales.
/// </remarks>
public class UserDomain : IIdentifiable<ulong>, ISoftDeletable
{
    /// <summary>
    /// Obtiene o establece el identificador único del usuario.
    /// </summary>
    /// <value>Identificador numérico del usuario. Campo obligatorio.</value>
    public required ulong Id { get; set; }

    /// <summary>
    /// Obtiene o establece el primer nombre del usuario.
    /// </summary>
    /// <value>Primer nombre del usuario. Campo obligatorio.</value>
    public required string FirstName { get; set; }

    /// <summary>
    /// Obtiene o establece el apellido paterno del usuario.
    /// </summary>
    /// <value>Apellido paterno del usuario. Campo obligatorio.</value>
    public required string FatherLastName { get; set; }

    /// <summary>
    /// Obtiene o establece la dirección de correo electrónico del usuario.
    /// </summary>
    /// <value>Email del usuario. Campo obligatorio.</value>
    public required string Email { get; set; }

    /// <summary>
    /// Obtiene o establece la contraseña del usuario (generalmente hasheada).
    /// </summary>
    /// <value>Contraseña del usuario. Campo obligatorio.</value>
    public required string Password { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación del usuario en el sistema.
    /// </summary>
    /// <value>Timestamp de cuando se creó el usuario. Campo obligatorio.</value>
    public required DateTime CreatedAt { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la última modificación del usuario.
    /// </summary>
    /// <value>Timestamp de cuando se modificó por última vez el usuario. Campo obligatorio.</value>
    public required DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Obtiene o establece el estado de activación del usuario.
    /// </summary>
    /// <value>
    /// true si el usuario está activo; false si está inactivo.
    /// Valor por defecto: true.
    /// </value>
    public bool Active { get; set; } = true;

    /// <summary>
    /// Obtiene o establece el tipo o rol del usuario dentro del sistema.
    /// </summary>
    /// <value>
    /// Rol del usuario definido por <see cref="UserType"/>.
    /// Valor por defecto: STUDENT.
    /// </value>
    public UserType Role { get; set; } = UserType.STUDENT;

    /// <summary>
    /// Obtiene o establece el segundo nombre del usuario (opcional).
    /// </summary>
    /// <value>
    /// Optional que contiene el segundo nombre si está presente,
    /// o None si no existe. Valor por defecto: None.
    /// </value>
    public Optional<string> MidName { get; set; } = Optional<string>.None();

    /// <summary>
    /// Obtiene o establece el apellido materno del usuario (opcional).
    /// </summary>
    /// <value>
    /// Optional que contiene el apellido materno si está presente,
    /// o None si no existe. Valor por defecto: None.
    /// </value>
    public Optional<string> MotherLastname { get; set; } = Optional<string>.None();
}
