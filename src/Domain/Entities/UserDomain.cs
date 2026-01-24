using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Representa un usuario del sistema con su información completa y metadatos.
/// </summary>
/// <remarks>
/// Encapsula todos los datos de un usuario, incluyendo información personal, credenciales,
/// estado, rol y metadatos de auditoría.
/// </remarks>
public class UserDomain
{
    /// <summary>
    /// Obtiene o establece el identificador único del usuario.
    /// </summary>
    public required ulong Id { get; set; }

    /// <summary>
    /// Obtiene o establece el primer nombre del usuario.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Obtiene o establece el apellido paterno del usuario.
    /// </summary>
    public required string FatherLastname { get; set; }

    /// <summary>
    /// Obtiene o establece la dirección de correo electrónico del usuario (debe ser única).
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Obtiene o establece la contraseña hasheada del usuario.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación de la cuenta de usuario.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la última modificación de los datos del usuario.
    /// </summary>
    public required DateTimeOffset ModifiedAt { get; set; }

    /// <summary>
    /// Obtiene o establece el estado de activación del usuario.
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// Obtiene o establece el rol del usuario dentro del sistema.
    /// </summary>
    public UserType Role { get; set; } = UserType.STUDENT;

    /// <summary>
    /// Obtiene o establece el segundo nombre del usuario (opcional).
    /// </summary>
    public string? MidName { get; set; }

    /// <summary>
    /// Obtiene o establece el apellido materno del usuario (opcional).
    /// </summary>
    public string? MotherLastname { get; set; }

public string FullName => string.Join(" ", 
    new[] { FatherLastname, MotherLastname, FirstName, MidName }
    .Where(s => !string.IsNullOrWhiteSpace(s)));
}
