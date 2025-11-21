using Application.DTOs.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.DTOs.Users;

/// <summary>
/// Representa los datos para actualizar la información de un usuario existente en el sistema.
/// </summary>
/// <remarks>
/// Esta clase encapsula la información que puede ser modificada de un usuario,
/// incluyendo datos personales, credenciales y estado. Todos los campos principales
/// son obligatorios para garantizar la integridad de la actualización.
/// </remarks>
public sealed record UserUpdateDTO
{
    /// <summary>
    /// Obtiene o establece el identificador único del usuario a actualizar.
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
    public required string FatherLastname { get; set; }

    /// <summary>
    /// Obtiene o establece la dirección de correo electrónico del usuario.
    /// </summary>
    /// <value>Email del usuario. Campo obligatorio.</value>
    public required string Email { get; set; }

    /// <summary>
    /// Obtiene o establece la contraseña del usuario (generalmente hasheada).
    /// </summary>
    /// <value>Contraseña del usuario. Campo obligatorio.</value>
    /// <remarks>
    /// Este campo debe contener la contraseña hasheada, no en texto plano.
    /// Para operaciones de cambio de contraseña, considera usar un campo separado.
    /// </remarks>
    public required string Password { get; set; }

    /// <summary>
    /// Obtiene o establece el estado de activación del usuario.
    /// </summary>
    /// <value>
    /// true si el usuario está activo; false si está inactivo.
    /// Valor por defecto: true.
    /// </value>
    public required bool Active { get; set; }

    public required UserType Role { get; set; }

    /// <summary>
    /// Obtiene o establece el segundo nombre del usuario (opcional).
    /// </summary>
    /// <value>
    /// Optional que contiene el segundo nombre si está presente,
    /// o None si no se desea modificar este campo.
    /// Valor por defecto: None.
    /// </value>
    public Optional<string> MidName { get; set; } = Optional<string>.None();

    /// <summary>
    /// Obtiene o establece el apellido materno del usuario (opcional).
    /// </summary>
    /// <value>
    /// Optional que contiene el apellido materno si está presente,
    /// o None si no se desea modificar este campo.
    /// Valor por defecto: None.
    /// </value>
    public Optional<string> MotherLastname { get; set; } = Optional<string>.None();

    /// <summary>
    /// DTO que representa el usuario que ejecuta la accion
    /// </summary>
    public required Executor Executor { get; set; }
}
