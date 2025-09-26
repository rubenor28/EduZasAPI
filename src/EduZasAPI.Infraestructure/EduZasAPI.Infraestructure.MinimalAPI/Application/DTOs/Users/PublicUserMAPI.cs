using EduZasAPI.Domain.Users;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Users;

/// <summary>
/// Representa la información pública de un usuario para ser expuesta externamente.
/// </summary>
/// <remarks>
/// Esta clase encapsula los datos de usuario que pueden ser compartidos de forma segura
/// en respuestas API o interfaces públicas. Incluye información básica de identificación
/// pero está diseñada para evitar exponer datos sensibles o internos del sistema.
/// </remarks>
public class PublicUserMAPI
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
    /// Obtiene o establece el segundo nombre del usuario (opcional).
    /// </summary>
    /// <value>
    /// Optional que contiene el segundo nombre si está presente,
    /// o null si no se proporciona. Valor por defecto: null.
    /// </value>
    public string? MidName { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el apellido paterno del usuario.
    /// </summary>
    /// <value>Apellido paterno del usuario. Campo obligatorio.</value>
    public required string FatherLastName { get; set; }

    /// <summary>
    /// Obtiene o establece el apellido materno del usuario (opcional).
    /// </summary>
    /// <value>
    /// Optional que contiene el apellido materno si está presente,
    /// o null si no se proporciona. Valor por defecto: null.
    /// </value>
    public string? MotherLastname { get; set; } = null;

    /// <summary>
    /// Obtiene o establece la dirección de correo electrónico del usuario.
    /// </summary>
    /// <value>Email del usuario. Campo obligatorio.</value>
    public required string Email { get; set; }

    /// <summary>
    /// Obtiene o establece el tipo o rol del usuario dentro del sistema.
    /// </summary>
    /// <value>
    /// Rol del usuario definido por <see cref="UserType"/>. 
    /// Valor por defecto: STUDENT.
    /// </value>
    public int Role { get; set; } = 0;
}
