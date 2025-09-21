namespace EduZasAPI.Domain.Enums.Users;

/// <summary>
/// Enumera los tipos de roles o perfiles de usuario disponibles en el sistema.
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
