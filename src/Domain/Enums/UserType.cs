namespace Domain.Enums;

/// <summary>
/// Define los roles que un usuario puede tener dentro de la aplicación.
/// </summary>
public enum UserType
{
    /// <summary>
    /// Representa un usuario con perfil de estudiante.
    /// </summary>
    STUDENT = 0,

    /// <summary>
    /// Representa un usuario con perfil de profesor o docente.
    /// </summary>
    PROFESSOR = 1,

    /// <summary>
    /// Representa un usuario con perfil de administrador del sistema.
    /// </summary>
    ADMIN = 2,
}
