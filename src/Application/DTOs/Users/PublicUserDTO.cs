namespace Application.DTOs.Users;

/// <summary>
/// Representa la información pública de un usuario para ser expuesta externamente.
/// </summary>
/// <remarks>
/// Esta clase encapsula los datos de usuario que pueden ser compartidos de forma segura
/// en respuestas API o interfaces públicas. Incluye información básica de identificación
/// pero está diseñada para evitar exponer datos sensibles o internos del sistema.
/// </remarks>
public sealed record PublicUserDTO
{
    /// <summary>
    /// Obtiene o establece el identificador único del usuario.
    /// </summary>
    /// <value>Identificador numérico del usuario. Campo obligatorio.</value>
    public required ulong Id { get; init; }

    public required bool Active { get; init; }

    /// <summary>
    /// Obtiene o establece el primer nombre del usuario.
    /// </summary>
    /// <value>Primer nombre del usuario. Campo obligatorio.</value>
    public required string FirstName { get; init; }

    /// <summary>
    /// Obtiene o establece el segundo nombre del usuario (opcional).
    /// </summary>
    /// <value>
    /// Segundo nombre del usuario. Valor por defecto: null.
    /// </value>
    public string? MidName { get; init; } = null;

    /// <summary>
    /// Obtiene o establece el apellido paterno del usuario.
    /// </summary>
    /// <value>Apellido paterno del usuario. Campo obligatorio.</value>
    public required string FatherLastname { get; init; }

    /// <summary>
    /// Obtiene o establece el apellido materno del usuario (opcional).
    /// </summary>
    /// <value>
    /// Apellido materno del usuario. Valor por defecto: null.
    /// </value>
    public string? MotherLastname { get; init; } = null;

    /// <summary>
    /// Obtiene o establece la dirección de correo electrónico del usuario.
    /// </summary>
    /// <value>Email del usuario. Campo obligatorio.</value>
    public required string Email { get; init; }

    /// <summary>
    /// Obtiene o establece el tipo o rol del usuario dentro del sistema.
    /// </summary>
    /// <value>
    /// Rol del usuario definido por <see cref="UserType"/>.
    /// Valor por defecto: STUDENT.
    /// </value>
    public required ulong Role { get; init; }
}
