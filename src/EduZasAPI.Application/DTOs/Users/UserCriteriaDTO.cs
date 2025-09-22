namespace EduZasAPI.Application.DTOs.Users;

using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Domain.Enums.Users;
using EduZasAPI.Application.DTOs.Common;

/// <summary>
/// Representa los criterios de búsqueda y filtrado para consultas de usuarios.
/// </summary>
/// <remarks>
/// Esta clase implementa la interfaz <see cref="ICriteria"/> y proporciona
/// filtros opcionales para todos los campos de la entidad User.
/// Utiliza <see cref="Optional{T}"/> para indicar qué campos deben ser filtrados
/// y cuáles deben ser ignorados en la consulta.
/// </remarks>
public class UserCriteriaDTO : ICriteriaDTO
{
    /// <summary>
    /// Obtiene o establece el filtro opcional para el estado de activación del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el valor booleano para filtrar por estado activo/inactivo,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public Optional<bool> Active { get; set; } = Optional<bool>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para el tipo o rol del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el <see cref="UserType"/> para filtrar por rol,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public Optional<UserType> Role { get; set; } = Optional<UserType>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para el primer nombre del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el primer nombre para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public Optional<StringQueryDTO> FirstName { get; set; } = Optional<StringQueryDTO>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para el segundo nombre del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el segundo nombre para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public Optional<StringQueryDTO> MidName { get; set; } = Optional<StringQueryDTO>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para el apellido paterno del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el apellido paterno para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public Optional<StringQueryDTO> FatherLastName { get; set; } = Optional<StringQueryDTO>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para el apellido materno del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el apellido materno para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public Optional<StringQueryDTO> MotherLastname { get; set; } = Optional<StringQueryDTO>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para el email del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene el email para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public Optional<StringQueryDTO> Email { get; set; } = Optional<StringQueryDTO>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para la contraseña del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene la contraseña para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public Optional<StringQueryDTO> Password { get; set; } = Optional<StringQueryDTO>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para la fecha de creación del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene la fecha de creación para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public Optional<DateTime> CreatedAt { get; set; } = Optional<DateTime>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para la fecha de modificación del usuario.
    /// </summary>
    /// <value>
    /// Optional que contiene la fecha de modificación para filtrar,
    /// o None si no se desea filtrar por este campo.
    /// </value>
    public Optional<DateTime> ModifiedAt { get; set; } = Optional<DateTime>.None();
}
