using Domain.Enums;
using Domain.ValueObjects;

namespace Application.DTOs.Users;

/// <summary>
/// Representa los criterios de búsqueda y filtrado para consultas de usuarios.
/// </summary>
/// <remarks>
/// Esta clase implementa la interfaz <see cref="ICriteria"/> y proporciona
/// filtros opcionales para todos los campos de la entidad User.
/// Utiliza <see cref="Optional{T}"/> para indicar qué campos deben ser filtrados
/// y cuáles deben ser ignorados en la consulta.
/// </remarks>
public sealed record UserCriteriaDTO : CriteriaDTO
{
    /// <summary>
    /// Obtiene o establece el filtro opcional para el estado de activación del usuario.
    /// </summary>
    /// <value>
    /// Valor booleano para filtrar por estado activo/inactivo.
    /// </value>
    public bool? Active { get; init; } 

    /// <summary>
    /// Obtiene o establece el filtro opcional para el tipo o rol del usuario.
    /// </summary>
    /// <value>
    /// Rol del usuario para filtrar.
    /// </value>
    public UserType? Role { get; init; } 

    /// <summary>
    /// Obtiene o establece el filtro opcional para el primer nombre del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el primer nombre para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public StringQueryDTO? FirstName { get; init; } 

    /// <summary>
    /// Obtiene o establece el filtro opcional para el segundo nombre del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el segundo nombre para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public StringQueryDTO? MidName { get; init; } 

    /// <summary>
    /// Obtiene o establece el filtro opcional para el apellido paterno del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el apellido paterno para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public StringQueryDTO? FatherLastname { get; init; } 

    /// <summary>
    /// Obtiene o establece el filtro opcional para el apellido materno del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el apellido materno para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public StringQueryDTO? MotherLastname { get; init; } 

    /// <summary>
    /// Obtiene o establece el filtro opcional para el email del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el email para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public StringQueryDTO? Email { get; init; } 

    /// <summary>
    /// Obtiene o establece el filtro opcional para la contraseña del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene la contraseña para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public StringQueryDTO? Password { get; init; } 

    /// <summary>
    /// Obtiene o establece el filtro opcional para la fecha de creación del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene la fecha de creación para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public DateTimeOffset? CreatedAt { get; init; } 

    /// <summary>
    /// Obtiene o establece el filtro opcional para la fecha de modificación del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene la fecha de modificación para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public DateTimeOffset? ModifiedAt { get; init; } 

    /// <summary>
    /// Obtiene o establece el filtro opcional para usuarios inscritos en una clase específica.
    /// </summary>
    /// <value>
    /// Optional que contiene el ID de la clase para filtrar usuarios inscritos,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public string? EnrolledInClass { get; init; } 

    /// <summary>
    /// Obtiene o establece el filtro opcional para usuarios que enseñan en una clase específica.
    /// </summary>
    /// <value>
    /// Optional que contiene el ID de la clase para filtrar usuarios que enseñan,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public string? TeachingInClass { get; init; } 
}
