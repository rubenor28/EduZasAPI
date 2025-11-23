using Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.Users;

/// <summary>
/// Representa los criterios de búsqueda y filtrado para consultas de usuarios.
/// </summary>
/// <remarks>
/// Esta clase implementa la interfaz <see cref="ICriteria"/> y proporciona
/// filtros opcionales para todos los campos de la entidad User.
/// Utiliza <see cref="Optional{T}"/> para indicar qué campos deben ser filtrados
/// y cuáles deben ser ignorados en la consulta.
/// </remarks>
public sealed record UserCriteriaMAPI : CriteriaDTO
{
    /// <summary>
    /// Obtiene o establece el filtro opcional para el estado de activación del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el valor booleano para filtrar por estado activo/inactivo,
    /// o null si no se desea filtrar por este campo.
    /// </value>
    public bool? Active { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el filtro opcional para el tipo o rol del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el <see cref="UserType"/> para filtrar por rol,
    /// o null si no se desea filtrar por este campo.
    /// </value>
    public uint? Role { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el filtro opcional para el primer nombre del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el primer nombre para filtrar,
    /// o null si no se desea filtrar por este campo.
    /// </value>
    public StringQueryMAPI? FirstName { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el filtro opcional para el segundo nombre del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el segundo nombre para filtrar,
    /// o null si no se desea filtrar por este campo.
    /// </value>
    public StringQueryMAPI? MidName { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el filtro opcional para el apellido paterno del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el apellido paterno para filtrar,
    /// o null si no se desea filtrar por este campo.
    /// </value>
    public StringQueryMAPI? FatherLastname { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el filtro opcional para el apellido materno del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el apellido materno para filtrar,
    /// o null si no se desea filtrar por este campo.
    /// </value>
    public StringQueryMAPI? MotherLastname { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el filtro opcional para el email del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el email para filtrar,
    /// o null si no se desea filtrar por este campo.
    /// </value>
    public StringQueryMAPI? Email { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el filtro opcional para la contraseña del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene la contraseña para filtrar,
    /// o null si no se desea filtrar por este campo.
    /// </value>
    public StringQueryMAPI? Password { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el filtro opcional para usuarios inscritos en una clase específica.
    /// </summary>
    public string? EnrolledInClass { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el filtro opcional para usuarios que enseñan en una clase específica.
    /// </summary>
    public string? TeachingInClass { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el filtro opcional para la fecha de creación del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene la fecha de creación para filtrar,
    /// o null si no se desea filtrar por este campo.
    /// </value>
    public DateTime? CreatedAt { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el filtro opcional para la fecha de modificación del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene la fecha de modificación para filtrar,
    /// o null si no se desea filtrar por este campo.
    /// </value>
    public DateTime? ModifiedAt { get; set; } = null;
}
