namespace Application.DTOs.Users;

/// <summary>
/// Representa las credenciales de autenticación de un usuario para el proceso de login.
/// </summary>
/// <remarks>
/// Esta estructura inmutable encapsula la información de autenticación requerida
/// para verificar la identidad de un usuario en el sistema. Ambos campos son
/// obligatorios para realizar el proceso de autenticación.
/// </remarks>
public sealed record UserCredentialsDTO
{
    /// <summary>
    /// Obtiene o establece la dirección de correo electrónico del usuario.
    /// </summary>
    /// <value>Email del usuario utilizado para identificarse. Campo obligatorio.</value>
    public required string Email { get; set; }

    /// <summary>
    /// Obtiene o establece la contraseña del usuario para autenticación.
    /// </summary>
    /// <value>Contraseña del usuario en texto plano. Campo obligatorio.</value>
    /// <remarks>
    /// Esta contraseña deberá ser verificada contra la versión hasheada almacenada
    /// en el sistema durante el proceso de autenticación.
    /// </remarks>
    public required string Password { get; set; }
}
